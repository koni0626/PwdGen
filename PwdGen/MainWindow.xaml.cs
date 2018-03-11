using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/* SHA256を使用するためのクラス */
using System.Security.Cryptography;

namespace PwdGen
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string PWDTYPE1 = "英数字特殊記号";
        public const string PWDTYPE2 = "英数字だけ";
        public const string PWDTYPE3 = "アルファベットだけ";
        public const int PWDTYPE_DEFAULT_TYPE_INDEX = 0;
        public const int PWDLEN_MIN = 6;
        public const int PWDLEN_MAX = 256;
        public const int PWDLEN_DEFAULT = 20;
        public MainWindow()
        {
            InitializeComponent();
            InitComboBox();
        }

        /* パスワード作成ボタンを押したときにコールされる */
        private void GenButton_Click(object sender, RoutedEventArgs e)
        {
            int PwdType = ComboBoxPwdType.SelectedIndex;
            int PwdLenIndex = ComboBoxPwdLen.SelectedIndex;
            int PwdLen = Int32.Parse(ComboBoxPwdLen.Items[PwdLenIndex].ToString());
            if(NormalPwdTextBox.Text.Length == 0)
            {
                MessageBox.Show("パスワードは1文字以上指定してください");
                return;
            }
            NewPwdTextBox.Text = CreateNewPasswd(NormalPwdTextBox.Text, PwdType, PwdLen);

        }

        /* コンボボックスを初期化します */
        private void InitComboBox()
        {
            ComboBoxPwdType.Items.Add(PWDTYPE1);
            ComboBoxPwdType.Items.Add(PWDTYPE2);
            ComboBoxPwdType.Items.Add(PWDTYPE3);

            for (int i = PWDLEN_MIN; i <= PWDLEN_MAX; i++)
            {
                ComboBoxPwdLen.Items.Add(i);
            }
            ComboBoxPwdType.SelectedIndex = PWDTYPE_DEFAULT_TYPE_INDEX;
            ComboBoxPwdLen.SelectedIndex = PWDLEN_DEFAULT - PWDLEN_MIN;
        }

        /* パスワードを生成する */
        private string CreateNewPasswd(string OldPasswd,int PwdType,int PwdLen)
        {
            string strNewPasswd;
            byte[] hash256Value = null;
            byte[] AllValue = new byte[32 * 8];
 
            // パスワードをUTF-8エンコードでバイト配列として取り出す
            byte[] byteValues = Encoding.UTF8.GetBytes(OldPasswd);

            // SHA256のハッシュ値を計算する
            SHA256 crypto256 = new SHA256CryptoServiceProvider();
            for (int i = 0; i < 256 / 32; i++)
            {
                hash256Value = crypto256.ComputeHash(byteValues);
                byteValues = hash256Value;
                byteValues.CopyTo(AllValue, 32 * i);
            }
            /* ハッシュ値を変換する */
            strNewPasswd = PasswdGen(AllValue, PwdType,PwdLen);
            return strNewPasswd;
        }

        /* ハッシュ値を指定されたタイプと長さの文字列に変換する */
        private string PasswdGen(byte[] hash256Value,int PwdType,int PwdLen)
        {
            StringBuilder hashedText = new StringBuilder();
            string PwdType1 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&()-=^~|@`{[}]*+;:_?><,.";
            string PwdType2 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string PwdType3 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string PwdTypeTable;
            switch(PwdType)
            {
                case 0:
                    PwdTypeTable = PwdType1;
                    break;
                case 1:
                    PwdTypeTable = PwdType2;
                    break;
                case 2:
                    PwdTypeTable = PwdType3;
                    break;
                default:
                    PwdTypeTable = PwdType1;
                    break;
            }

            for (int i = 0; i < PwdLen;i++)
            { 
                int pos = hash256Value[i] % PwdTypeTable.Length;
                hashedText.Append(PwdTypeTable[pos]);
            }

            return hashedText.ToString();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPwdTextBox.Text.Length == 0)
            {
                MessageBox.Show("パスワードは1文字以上指定してください");
                return;
            }

            Clipboard.SetData(DataFormats.Text, (Object)NewPwdTextBox.Text);
            MessageBox.Show("クリップボードにパスワードを送りました");
        }
    }
}
