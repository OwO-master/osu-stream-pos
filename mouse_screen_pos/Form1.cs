using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Windows.Input;
using System.Net;

namespace mouse_screen_pos
{
    public partial class Form1 : Form
    {
        public char[] leftkey = { 'k', 'e', 'y', 'O', 's', 'u', 'L', 'e', 'f', 't', ' ', '=', ' ' };
        public char[] rightkey = { 'k', 'e', 'y', 'O', 's', 'u', 'R', 'i', 'g', 'h', 't', ' ', '=', ' ' };
        public char[] keylist = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public string msgannouce;
        public string leftclick;
        public string rightclick;
        public Key lc = Key.NoName;
        public Key rc = Key.NoName;
        private int alc = 0;
        private int arc = 0;
        private string nowtime = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Import the osu! profile" + '\n' + "or open a recent profile.";
            textBox1.Visible = false;
            if (osu_stream_pos.Properties.Settings.Default.Profile == "")
            {

            }
            else
            {
                nothingToolStripMenuItem.Text = osu_stream_pos.Properties.Settings.Default.Profile;
            }
            timer.Enabled = false;
            toolStripStatusLabel1.Text = osu_stream_pos.Properties.Settings.Default.X.ToString();
            toolStripStatusLabel2.Text = osu_stream_pos.Properties.Settings.Default.Y.ToString();
            toolStripStatusLabel3.Text = "Waiting for profile";
            string apicode = "";
            string namecode = "";
            DialogResult information = InputBox("Input your osu! API information", "Name", "API", ref namecode, ref apicode);
            if (information == DialogResult.OK)
            {
                if (namecode == "" || apicode == "")
                {
                    MessageBox.Show("Please input the information.");
                    Application.Restart();
                }
                else
                {
                    using (WebClient API = new WebClient())
                    {
                        string url = OsuGetType.GetUser() + OsuAPI.USERNAME(namecode) + "&m=0&" + OsuAPI.API(apicode);
                        string content = API.DownloadString(url);
                        content = content.Replace("\",\"", Environment.NewLine);
                        content = content.Replace("\":\"", " : ");
                        MessageBox.Show(content + "@@@@@");
                    }
                    Text = namecode;
                }
            }
            else if (information == DialogResult.Cancel)
            {
                Application.Exit();
            }
        }
        
        void Nowtime(string tiem, Key press, string dir)
        {
            textBox1.AppendText(Environment.NewLine + "@" + DateTime.Now.ToString("HH:mm:ss:fff") + "  " + "#" + System.Windows.Forms.Cursor.Position.X + "," + System.Windows.Forms.Cursor.Position.Y + "  " + press + " is " + dir + ".");
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            label1.Text = "PosX : " + System.Windows.Forms.Cursor.Position.X + '\n' + "PosY : " + System.Windows.Forms.Cursor.Position.Y + '\n' + openFileDialog1.FileName;
            label2.Text = Keyboard.IsKeyDown(lc).ToString();
            label3.Text = Keyboard.IsKeyDown(rc).ToString();

            if (alc == 0)
            {
                if (Keyboard.IsKeyDown(lc) == true)
                {
                    alc = 1;
                    toolStripStatusLabel1.Text = Convert.ToString(Convert.ToInt32(toolStripStatusLabel1.Text) + 1);
                    pictureBox1.BackColor = Color.Red;
                    Nowtime(nowtime, lc, "down");
                }
            } else if (alc == 1)
            {
                if (Keyboard.IsKeyDown(lc) == false)
                {
                    alc = 0;
                    pictureBox1.BackColor = Color.Transparent;
                    Nowtime(nowtime, lc, "up");
                }
            }
            if (arc == 0)
            {
                if (Keyboard.IsKeyDown(rc) == true)
                {
                    arc = 1;
                    toolStripStatusLabel2.Text = Convert.ToString(Convert.ToInt32(toolStripStatusLabel2.Text) + 1);
                    pictureBox2.BackColor = Color.Red;
                    Nowtime(nowtime, rc, "down");
                }
            }
            else if (arc == 1)
            {
                if (Keyboard.IsKeyDown(rc) == false)
                {
                    arc = 0;
                    pictureBox2.BackColor = Color.Transparent;
                    Nowtime(nowtime, rc, "up");
                }
            }

        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Detect_Keystate(openFileDialog1.FileName);
            toolStripStatusLabel3.Text = "Got the file. Looking at the keybind.";
        }

        private void Detect_Keystate(string loc)
        {
            string[] profile2 = File.ReadAllLines(loc);
            int arraycount = profile2.Count();
            toolStripStatusLabel3.Text = "Found " + arraycount + " line(s) of setting in the file.";
            for (int i = 0; i < arraycount; i++)
            {
                if (profile2[i].Contains("keyOsuLeft"))
                {
                    leftclick = profile2[i];
                    toolStripStatusLabel3.Text = "Found \"keyOsuLeft\" at No." + i + " line!";
                }
                else if (profile2[i].Contains("keyOsuRight"))
                {
                    rightclick = profile2[i];
                    toolStripStatusLabel3.Text = "Found \"keyOsuRight\" at No." + i + " line!";
                }
            }
            leftclick = leftclick.TrimStart(leftkey);
            rightclick = rightclick.TrimStart(rightkey);
            msgannouce += "Your left key is : " + leftclick + '\n' + "Your right key is : " + rightclick + '\n' + '\n' + "Is this settings as same as your osu keybind?";
            lc = KeyInterop.KeyFromVirtualKey(Convert.ToInt32(leftclick[0]));
            rc = KeyInterop.KeyFromVirtualKey(Convert.ToInt32(rightclick[0]));
            MessageBox.Show(msgannouce);
            textBox1.Visible = true;
            timer.Enabled = true;
        }

        private void OpenProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            msgannouce = "";
            openFileDialog1.ShowDialog();
        }

        private void nothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nothingToolStripMenuItem.Text == "Nothing")
            {

            }
            else
            {
                msgannouce = "";
                Detect_Keystate(osu_stream_pos.Properties.Settings.Default.Profile);
            }
        }

        private void RESETToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Font = new Font("x14y24pxHeadUpDaisy", 24, FontStyle.Bold);
            textBox1.Font = new Font("x14y24pxHeadUpDaisy", 10, FontStyle.Regular);
        }

        private void SETToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult pickfont = fontDialog1.ShowDialog();
            if (pickfont == DialogResult.OK)
            {
                Font = new Font(fontDialog1.Font.FontFamily, fontDialog1.Font.Size, fontDialog1.Font.Style);
            }
        }

        private void RESETToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BackColor = SystemColors.Control;
        }

        private void SETToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult pickcolor = colorDialog1.ShowDialog();
            if (pickcolor == DialogResult.OK)
            {
                BackColor = colorDialog1.Color;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.FileName == "")
            {

            }
            else
            {
                osu_stream_pos.Properties.Settings.Default["Profile"] = openFileDialog1.FileName;
            }
            osu_stream_pos.Properties.Settings.Default["X"] = Convert.ToInt32(toolStripStatusLabel1.Text);
            osu_stream_pos.Properties.Settings.Default["Y"] = Convert.ToInt32(toolStripStatusLabel2.Text);
            osu_stream_pos.Properties.Settings.Default.Save();
            savelog();
            Close();
        }

        void savelog()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, textBox1.Text);
            }
        }

        private void resetCounterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = Convert.ToString(0);
            toolStripStatusLabel2.Text = Convert.ToString(0);
        }

        private void ValxeEveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=ycQ2QIyE-Uw");
        }

        private static DialogResult InputBox(string title, string nameText, string apiText, ref string nvalue, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            Label label2 = new Label();
            TextBox textBox = new TextBox();
            TextBox textbox2 = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            LinkLabel osuapiurl = new LinkLabel();

            form.Text = title;
            label.Text = nameText;
            label2.Text = apiText;
            textBox.Text = nvalue;
            textbox2.Text = value;
            osuapiurl.Text = "Get API";
            osuapiurl.LinkClicked += new LinkLabelLinkClickedEventHandler(Osuevent);

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            label2.SetBounds(9, 70, 372, 13);
            textbox2.SetBounds(12, 86, 372, 20);
            osuapiurl.SetBounds(9, 122, 75, 23);
            buttonOk.SetBounds(228, 122, 75, 23);
            buttonCancel.SetBounds(309, 122, 75, 23);

            label.AutoSize = true;
            label2.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textbox2.Anchor = textbox2.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 157);
            form.Controls.AddRange(new Control[] { label, textBox, label2, textbox2, osuapiurl, buttonOk/*,buttonCancel*/ });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            nvalue = textBox.Text;
            value = textbox2.Text;
            return dialogResult;
        } //InputBox Code from Y2J. Thanks! @ https://dotblogs.com.tw/aquarius6913/2014/09/03/146444

        static void Osuevent(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://osu.ppy.sh/p/api");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void GCTimer_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }
    }

    public class OsuAPI
    {
        internal static string API(string apiinput)
        {
            apiinput = "k=" + apiinput;
            return apiinput;
        }      

        internal static string USERNAME(string name)
        {
            name = "u=" + name;
            return name;
        }

        internal static void Beatmap()
        {

        }
    }
    
    public class OsuGetType
    {
        internal static string GetUser()
        {
            return "https://osu.ppy.sh/api/get_user?";
        }
    }
}


