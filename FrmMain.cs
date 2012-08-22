using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ShowLib;

namespace SrvSinc
{
    public partial class FrmMain : Form
    {
        private string[] args;
        private bool mesclarSwitch = false;
        //"C:\VOB_view\34613e-Processo\01-Sistema\06-Implementacao\01-Aplicacao\Web\js\Hint.js"  "C:\VOB_view\34613e-Processo\01-Sistema\06-Implementacao\01-Aplicacao\Web" "C:\teste_server\www" 
        
        // C:\Dev\SrvSinc\SrvSinc\bin\Debug\SrvSinc.exe "$(FULL_CURRENT_PATH)" "C:\VOB_view\34613e-Processo\02-Projetos\Projetos Resumidos e Sumarios\88-e-Processo-088\06-Implementacao\01-Aplicacao\Web" "X:\Inetpub\wwwroot\e-Processo" -m
        //C:\Dev\SrvSinc\SrvSinc\bin\Debug\SrvSinc.exe "C:\VOB_view\34613e-Processo\01-Sistema\06-Implementacao\01-Aplicacao\Web\sessao\TelaInformarResultadoJulgamento.asp" "C:\VOB_view\34613e-Processo\01-Sistema\06-Implementacao\01-Aplicacao\Web" \"\\10.50.241.160\d$\WebSites\Sunac_Hom_34613_EProcesso" -m
        public FrmMain(string[] args)
        {
            InitializeComponent();
            //MessageBox.Show(args[0] + Environment.NewLine + args[1] + Environment.NewLine + args[2] + Environment.NewLine + args[3]);
            //MessageBox.Show(args[0] + Environment.NewLine + args[1] + Environment.NewLine + args[2]);
            for (int i = 0; i < args.Length; ++i)
            {
                args[i] = args[i].Replace("\"", "");
                //args[i] = args[i].ToLower();
            }
            this.args = args;
            int returnCode = validacao();
            if (returnCode != 0)
            {
                System.Environment.Exit(returnCode);
            }
            if (!mesclarSwitch)
                returnCode = copiar();
            else
                returnCode = mesclar();
            System.Environment.Exit(returnCode);
        }

        private int validacao()
        {
            if (args.Length < 3)
            {
                string uso = "Uso: \n\n SrvSinc.exe \"$(FULL_CURRENT_PATH)\" \"Diretorio 'Web' da máquina local\"  \"Diretorio 'Web' do servidor de página\" [-m] (opcional, usado para mesclagem dos arquivos). \n\n";
                uso += "Exemplo: \n\n c:\\windows\\SrvSinc.exe \"$(FULL_CURRENT_PATH)\" \"C:\\VOB_view\\34613e-Processo\\01-Sistema\\06-Implementacao\\01-Aplicacao\\Web\"  \"\\\\10.50.241.160\\d$\\WebSites\\Sunac_Hom_34613_EProcesso\"";
                MessageBox.Show(uso);
                return -1;
            }else{
                if (args.Length == 3)
                {
                    mesclarSwitch = false;
                    return 0;
                }
                else
                {
                    if (args.Length == 4 && args[3].ToLower() != "-m")
                    {
                        MessageBox.Show("Uso: SrvSinc.exe arquivo \"Diretorio 'Web' da máquina local\"  \"Diretorio 'Web' do servidor de página\" [-m] (opcional, usado para mesclagem dos arquivos).");
                        return -1;
                    }
                    else
                    {
                        mesclarSwitch = true;
                    }
                }
            }

            if (args.Length > 4)
            {
                MessageBox.Show("Erro! Mais de 4 parâmetros!!! Uso: SrvSinc.exe arquivo \"Diretorio 'Web' da máquina local\"  \"Diretorio 'Web' do servidor de página\" [-m] (opcional, usado para mesclagem dos arquivos).");
                return -1;
            }

            if (!File.Exists(args[0]))
            {
                MessageBox.Show("Não foi localizado o arquivo a ser copiado. " + args[0]);
                return -2;
            }

            if (!Directory.Exists(args[1]))
            {
                MessageBox.Show("Não foi localizado o diretório 'Web' da máquina local.");
                return -3;
            }

            if (!Directory.Exists(args[2]))
            {
                MessageBox.Show("Não foi localizado o diretório 'Web' do servidor.");
                return -4;
            }

            string offset = args[0].Replace(args[1], "");
            if (!Directory.Exists(Fcn.FilePath(args[2] + offset)))
            {
                MessageBox.Show("Não foi localizado o diretório " + Fcn.FilePath(args[2] + offset));
                return -5;
            }
            return 0;
        }


        private int mesclar()
        {
            bool mesclado = false;
            string offset = ReplaceInsensitive(args[0], args[1], "");
            //string offset = args[0].Replace(args[1], "");
            try
            {
                if (File.Exists(args[2] + offset))
                {
                    File.SetAttributes(args[2] + offset, FileAttributes.Normal);
                }
                string WinMergeU = Environment.GetEnvironmentVariable("PROGRAMFILES") + "\\WinMerge\\WinMergeU.exe";
                string argumentos = " /u  \"" + args[0] + "\"  \"" + (args[2] + offset) + "\"";
                Fcn.CommandExecute(WinMergeU, argumentos, "");
                mesclado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao fazer a mesclagem dos arquivos " + ex.Message);
                return -6;
            }
            if (mesclado)
            {
                File.SetAttributes(args[2] + offset, FileAttributes.ReadOnly);
            }
            return 0;
        }


        private int copiar(){
            bool copiado = false;
            string offset = ReplaceInsensitive(args[0], args[1], "");
            //string offset = args[0].Replace(args[1], "");

            try
            {
                if (File.Exists(args[2] + offset))
                {
                    File.SetAttributes(args[2] + offset, FileAttributes.Normal);
                }

                if (DateTime.Now - File.GetLastWriteTime(args[0]) > TimeSpan.FromSeconds(6))
                {
                    if (MessageBox.Show("O arquivo sendo editado foi modificado pela última vem em: " + File.GetLastWriteTime(args[0]) + " tem certeza que deseja publicar?", "Alerta!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        //MessageBox.Show("Copiar " + args[0] + " para " + args[2] + offset);
                        File.Copy(args[0], args[2] + offset, true);
                        copiado = true;
                    }
                }
                else
                {
                    //MessageBox.Show("Copiar " + args[0] + " para " + args[2] + offset);
                    File.Copy(args[0], args[2] + offset, true);
                    copiado = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao copiar o arquivo para o servidor " + ex.Message);
                return -6;
            }
            if (copiado)
            {
                File.SetAttributes(args[2] + offset, FileAttributes.ReadOnly);
                FrmNotification.Show("Arquivo publicado em: " + args[2] + offset);
            }
            return 0;
        }


        private static string ReplaceInsensitive(string original,  string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
    