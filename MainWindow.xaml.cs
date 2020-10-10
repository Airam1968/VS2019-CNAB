using System.Windows;
using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CNAB
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Boolean lOk = true;
            //
            edprogresso.Text += "\r\n";
            //Leitura do arquivo CSV de origem
            //try
            //{
            var listaorigem = File.ReadAllLines(@edarquivoorigem.Text, Encoding.Default)
                    .Select(a => a.Split(';'))
                    .Select(c => new linha()
                    {
                        entidade = c[0],
                        dtmatricula = c[1],
                        dataaceite = c[2],
                        aluno = c[3],
                        cpf = c[4],
                        endereco = c[5],
                        bairro = c[6],
                        email = c[7],
                        cep = c[8],
                        uf = c[9],
                        municipio = c[10],
                        vencimento = c[11],
                        nossonumero = c[12],
                        rsoriginal = c[13],
                        telefone = c[14],
                        venda = c[15],
                        parcela = c[16],
                        meiopagamento = c[17],
                        curso = c[18],
                        nascimento = c[19],
                        sexo = c[20],
                        mae = c[21],
                        pai = c[22],
                        rg = c[23],
                        orgao_exp = c[24],
                        dt_exp = c[25],
                        sit_mtr = c[26],
                        evol_mtr = c[27]
                    })
                    .ToList();
            listaorigem.RemoveAt(0);
            //
            edprogresso.Text = edprogresso.Text + "(" + listaorigem.Count().ToString() + ")" + " linhas adicionadas, sem o cabeçalho." + "\r\n";
            edprogresso.Text = edprogresso.Text + "\r\n";
            //
            Int16 contador = 0;
            foreach (var item in listaorigem)
            {
                edprogresso.Text = edprogresso.Text + contador.ToString() + " - " + item.aluno + " - " + item.nascimento + "\r\n";
                contador++;
            }

            //}
            //catch (Exception)
            //{
            //    lOk = false;
            //    MessageBox.Show("Não foi possível abrir ou ler o arquivo!", "Mensagem");
            //}
            //Processamento das linhas do CNAB
            if (lOk)
            {
                List<string> listadestino = new List<string>();
                string aux = new string(' ', 277);
                //Linha Header
                listadestino.Add("01" +
                                 "REMESSA" +
                                 "01" +
                                 "COBRANCA       " +
                                 "00000000000004123376" +
                                 "INOVA-EAD CONSULTORIA EAD LTDA" +
                                 "237" +
                                 "BRADESCO       " +
                                 "      " + //Data da Gravação do Arquivo
                                 "        " +
                                 "MX" +
                                 "0000001" +
                                 aux +
                                 "000001"
                                 );
                //Linha Detalhe
                contador = 2;
                string endereco = "";
                string nome = "";
                double vlrmora = 0;
                string vlrmora2 = "";
                foreach (var item in listaorigem)
                {
                    if (item.aluno != string.Empty)
                    {
                        vlrmora = Convert.ToDouble(item.parcela);
                        vlrmora = vlrmora * 0.00033;
                        vlrmora2 = Math.Truncate(vlrmora).ToString();
                        //
                        if (item.endereco.Length <= 40)
                        {
                            endereco = item.endereco;
                        }
                        else
                        {
                            endereco = item.endereco.Substring(0,39);
                        }
                        if (item.aluno.Length <= 40)
                        {
                            nome = item.aluno;
                        }
                        else
                        {
                            nome = item.aluno.Substring(0, 39);
                        }
                        //
                        listadestino.Add("1" + //Operação 1 pos
                                         "00000" + // Agência de Débito (opcional)  5 pos
                                         "0" + // Dígito da Agência de Débito (opcional) 1 pos
                                         "00000" + //Razão da Conta Corrente(opcional) 5 pos
                                         "0000000" + // Conta Corrente (opcional) 7 pos
                                         "0" + // Dígito da Conta Corrente (opcional) 1 pos
                                         //"0" + //zero 1 pos
                                         "0009" + //carteira
                                         "00436" + //agencia
                                         "0458418" + //conta
                                         "0" + //digito conta
                                         item.nossonumero.ToString().PadLeft(10, '0') + "               " + //nosso numero 10pos + 15 pos espaço
                                         //"1 0003765689 Z           " + //Identificação da Empresa Beneficiária no banco 25 pos
                                         "000" + //Código do Banco "237"
                                         "2" + //0 sem multa, 2 com multa
                                         "0200" + //percentual de Multa
                                         "00000000000" + //nosso numero calcular?
                                         "0" +
                                         "0000000000" +
                                         "2" +
                                         " " +
                                         "          " + //Identificação da Operação do Banco?
                                         " " +
                                         " " +
                                         "  " + //branco
                                         "01" + //Ocorrêmcia
                                         item.nossonumero.ToString().PadLeft(10, '0') + //Nosso Numero Inova
                                         //"0000000000" + //Documento Bradesco = Nosso Numero Inova | KS nosso numero=numero do documento
                                         item.vencimento.Substring(0, 2) + item.vencimento.Substring(3, 2) + item.vencimento.Substring(8, 2) + //Vencimento do Titulo = 6 pos
                                         Regex.Replace(item.rsoriginal, "[^0-9]", "").ToString().PadLeft(13, '0') + //valor do titulo sem o ponto 13 pos
                                         "000" +
                                         "00000" +
                                         "01" +
                                         "N" +
                                         DateTime.Now.ToString().Substring(0, 2) + DateTime.Now.ToString().Substring(3, 2) + DateTime.Now.ToString().Substring(8, 2) + //Emissão do Titulo = 6 pos
                                         "00" +
                                         "00" +
                                         Regex.Replace(vlrmora2, "[^0-9]", "").ToString().PadLeft(13, '0') + //valor da mora por dia de atraso
                                         //"0000000000000" + //valor da mora por dia de atraso
                                         "000000" +
                                         "0000000000000" + //valor do desconto
                                         "0000000000000" + //valor do IOF
                                         "0000000000000" + //valor do abatimento
                                         "01" + //02 CNPJ ou 01 CPF
                                         Regex.Replace(item.cpf, "[^0-9]", "").ToString().PadLeft(14, '0') + //CNPJ ou CPF 14 pos
                                         nome.ToString().PadRight(40, ' ') + //Nome 40 pos
                                         //item.aluno.ToString().PadRight(40, ' ') + //Nome 40 pos
                                         endereco.ToString().PadRight(40, ' ') + //Endereço 40 pos
                                         //item.endereco.ToString().PadRight(40, ' ') + //Endereço 40 pos
                                         "            " + //1a. Mensagem 12 posições?
                                         Regex.Replace(item.cep, "[^0-9]", "").ToString().PadLeft(8, '0') + //CEP 8pos
                                         "Ref. curso de pós-graduação EaD UCAM                        " + //Sacador ou Avalista ou 2a. Mensagem 60 posições
                                         contador.ToString().PadLeft(6, '0') //Sequencial do Registro 6 pos
                                         );
                        contador++;
                    }
                }
                //Linha Footer
                aux = new string(' ', 393);
                listadestino.Add("9" +
                                 aux +
                                 contador.ToString().PadLeft(6, '0') //Sequencial do Registro 6 pos
                                 );
                //Gravar Linhas no arquivo destino
                string arquivodestino = @edarquivodestino.Text;

                //string[] createText = { "Hello", "And", "java2s.com" };
                File.WriteAllLines(arquivodestino, listadestino, Encoding.UTF8);

            }
            //
            if (lOk)
            {
                MessageBox.Show("Processamento Concluído!", "Mensagem");
            }



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
            //enviroment.exit(0)
        }
    }
}
