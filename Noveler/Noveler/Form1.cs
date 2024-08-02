using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;

//json저장
using Newtonsoft.Json.Linq;

namespace Noveler
{
    public partial class Form1 : Form
    {
        int nowShowingSection = 0;
        //array의 list를 선언
        List<string[]> Nov = new List<string[]>() { new string[] { "", "" } };
        public Form1()
        {
            InitializeComponent();
        }

        //파일 오픈을 통해 진입한 경우

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            if (Program.startopen) { open(); }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            Nov[nowShowingSection][0] = textBox1.Text;
            refreshUi();
            ttmHasChanged = true;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Nov[nowShowingSection][0] = textBox1.Text;
                refreshUi();
                ttmHasChanged = true;
            }
        }

        private void 새구간추가ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savecontent();
            textBox1.Text = "";
            string[] addsector = new string[] { "", "" };
            Nov.Insert(nowShowingSection + 1, addsector);


            nowShowingSection = nowShowingSection + 1;
            refreshUi();
            ttmHasChanged = true;
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            savecontent();
            nowShowingSection = nowShowingSection + 1;
            if (nowShowingSection >= Nov.Count())
            {
                nowShowingSection = Nov.Count() - 1;
            }
            refreshUi();
        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            savecontent();
            nowShowingSection = nowShowingSection - 1;
            if (nowShowingSection < 0)
            {
                nowShowingSection = 0;
            }
            refreshUi();
        }


        private void txtbxcontent_Leave(object sender, EventArgs e)
        {
            savecontent();
        }
        private void txtbxcontent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Back)
            {
                savecontent();
            }
        }

        //내용 저장, 길이 갱신
        int exeptlength = 0;
        void savecontent()
        {
            int num;
            Nov[nowShowingSection][0] = textBox1.Text;
            Nov[nowShowingSection][1] = txtbxcontent.Text;
            num = txtbxcontent.Text.Length;
            txtbxlength.Text = num.ToString();
            txtbxfulllength.Text = (num + exeptlength).ToString();
        }
        //현재 섹터를 제외한 섹터들의 길이 합
        void calcexeptlength()
        {
            int sum = 0;
            for (int i = 0; i < Nov.Count(); i++)
            {
                if (i != nowShowingSection)
                {
                    sum = sum + Nov[i][1].Length;
                }
            }
            exeptlength = sum;
        }

        //refreshui함수
        void refreshUi()
        {
            textBox1.Text = Nov[nowShowingSection][0];
            txtbxcontent.Text = Nov[nowShowingSection][1];
            int NumberOfSectors = Nov.Count() - 1;
            chart1.Series[0].Points.Clear();
            for (int i = 0; i <= NumberOfSectors; i++)
            {
                chart1.Series[0].Points.AddXY(Nov[NumberOfSectors - i][0] + "구간", Nov[NumberOfSectors - i][1].Length);
            }
            textBox2.Text = (nowShowingSection + 1).ToString() + "번 구간";
            calcexeptlength();
            //길이 출력
            int num = txtbxcontent.Text.Length;
            txtbxlength.Text = num.ToString();
            txtbxfulllength.Text = (num + exeptlength).ToString();
        }

        private void 이전구간에병합ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nowShowingSection == 0)
            {
                return;
            }
            Nov[nowShowingSection - 1][1] = Nov[nowShowingSection - 1][1] + "\n\r" + Nov[nowShowingSection][1];
            Nov.RemoveAt(nowShowingSection);
            nowShowingSection = nowShowingSection - 1;
            refreshUi();
            ttmHasChanged = true;
        }

        private void 뒤구간에병합ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nowShowingSection == Nov.Count - 1)
            {
                return;
            }
            Nov[nowShowingSection + 1][1] = Nov[nowShowingSection][1] + "\n\r" + Nov[nowShowingSection + 1][1];
            Nov.RemoveAt(nowShowingSection);
            refreshUi();
            ttmHasChanged = true;
        }
        //이 아래는 저장과 로드
        //저장/로드 구조 갈아 엎기.
        //

        string savedirectory = "";
        private void 저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Nov[nowShowingSection][0] = textBox1.Text;
            Nov[nowShowingSection][1] = txtbxcontent.Text;
            if (savedirectory == "")
            {
                다른이름으로저장ToolStripMenuItem_Click(sender, e);
                return;
            }
            JObject jsonNovel = new JObject(new JProperty("content",Nov));
            try
            {
                using (StreamWriter sw = new StreamWriter(savedirectory))
                {
                    sw.Write(jsonNovel.ToString());
                }
            }
            catch 
            {
                MessageBox.Show("저장 실패");
            }
        }

        private void 다른이름으로저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Nov[nowShowingSection][0] = textBox1.Text;
            Nov[nowShowingSection][1] = txtbxcontent.Text;
            JObject jsonNovel = new JObject(new JProperty("content", Nov));

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "노벨러 파일|*.nvr|모든 파일|*.*";
            saveFileDialog1.FilterIndex = 1;
            // 대화상자를 닫기 전에 디렉토리를 이전에 선택한 디렉토리로
            // 복원한지의 여부를 나타납니다.
            saveFileDialog1.RestoreDirectory = true;
            // 확장명을 입력하지 않을 때, 자동으로 확장자를 추가할 수 있습니다.
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "nvr";

            // 저장할 위치의 초기 디렉토리를 설정합니다.
            // Environment.CurrentDirectory: 현재 디렉토리를 나타냅니다.
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Text = saveFileDialog1.FileName;
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    sw.Write(jsonNovel.ToString());
                }
            }
            savedirectory = saveFileDialog1.FileName;
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            calcexeptlength();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "노벨러 파일|*.nvr|모든 파일|*.*";
            ofd.FilterIndex = 1;  // 인덱스 1부터 시작
            ofd.Multiselect = false;

            if (exeptlength != 0 || txtbxcontent.Text != "") MessageBox.Show("기존 내용을 저장하지 않고 새 파일을 로드하면 기존 내용은 손실됩니다. 저장했는지 확인해 보세요.");
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.Text = ofd.FileName;   // 폼의 타이틀
                savedirectory = ofd.FileName;
                try
                {
                    using (StreamReader file = File.OpenText(ofd.FileName))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            JObject json = (JObject)JToken.ReadFrom(reader);
                            List<string> novcomponent = new List<string>();
                            int num = json["content"].Count();
                            if ((num % 2) == 1)
                            {
                                MessageBox.Show("파일 로드 실패(홀수 인덱스)");
                                return;
                            }
                            int hnum = num / 2;
                            Nov = new List<string[]>();
                            for (int i = 0; i < num; i++)
                            {
                                novcomponent.Add(json["content"][i].ToString());
                                Console.WriteLine(i);
                            }
                            Console.WriteLine("제이슨 리스트 가져오기 성공");
                            for (int i = 0; i < hnum; i++)
                            {
                                Nov.Add(new string[] { novcomponent[i], novcomponent[i + hnum] });
                            }
                            nowShowingSection = 0;
                            refreshUi();
                            ttmHasChanged = true;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("파일 로드 실패");
                }
            }
        }

        void open() 
        {
            this.Text = Program.sdirectory;
            savedirectory = Program.sdirectory;
            try
            {
                using (StreamReader file = File.OpenText(savedirectory))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject json = (JObject)JToken.ReadFrom(reader);
                        List<string> novcomponent = new List<string>();
                        int num = json["content"].Count();
                        if ((num % 2) == 1)
                        {
                            MessageBox.Show("파일 로드 실패(홀수 인덱스)");
                            return;
                        }
                        int hnum = num / 2;
                        Nov = new List<string[]>();
                        for (int i = 0; i < num; i++)
                        {
                            novcomponent.Add(json["content"][i].ToString());
                            Console.WriteLine(i);
                        }
                        Console.WriteLine("제이슨 리스트 가져오기 성공");
                        for (int i = 0; i < hnum; i++)
                        {
                            Nov.Add(new string[] { novcomponent[i], novcomponent[i + hnum] });
                        }
                        nowShowingSection = 0;
                        refreshUi();
                        ttmHasChanged = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("파일 로드 실패");
            }
        }
        
        //이 아래는 구간별 툴스트립버튼 제어

        ToolStripMenuItem[] sectorbutton;

        bool ttmHasChanged = false;
        private void RefreshSectorbutton(object sender, EventArgs e)
        {
            //변경 이후 처음 클릭이 아닐 시 바로 return한다.
            if (!ttmHasChanged) 
            {
                return;
            }
            //기존 메뉴아이템 삭제
            //메뉴아이템 배열 초기화
            sectorbutton = new ToolStripMenuItem[Nov.Count];
            this.toolStripSplitButton3.DropDownItems.Clear();
            //배열 할당 및 인스턴스화
            for (int i = 0; i < Nov.Count; i++)
            {
                //할당
                sectorbutton[i] = new ToolStripMenuItem();
                sectorbutton[i].ToolTipText = i.ToString();
                sectorbutton[i].Text = Nov[i][0];
                sectorbutton[i].Name = "stb" + i.ToString();
                sectorbutton[i].Click += new System.EventHandler(this.onSectoionselected);
                //인스턴스화
                //toolStripSplitButton3.Container.Add(sectorbutton[i]);

            }
            this.toolStripSplitButton3.DropDownItems.AddRange(sectorbutton);
            ttmHasChanged = false;
        }
        void onSectoionselected(object sender, EventArgs e) 
        {
            ToolStripMenuItem tm = (ToolStripMenuItem)sender;
            int i = int.Parse(tm.ToolTipText);
            nowShowingSection = i;
            refreshUi();
        }
        
        
        
        /// <summary>
        ///이 아래에서 익스포트 
        /// </summary>
        /// 
        private void txtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Nov[nowShowingSection][0] = textBox1.Text;
            Nov[nowShowingSection][1] = txtbxcontent.Text;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "텍스트파일|*.txt|모든 파일|*.*";
            saveFileDialog1.FilterIndex = 1;
            // 대화상자를 닫기 전에 디렉토리를 이전에 선택한 디렉토리로
            // 복원한지의 여부를 나타납니다.
            saveFileDialog1.RestoreDirectory = true;
            // 확장명을 입력하지 않을 때, 자동으로 확장자를 추가할 수 있습니다.
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "txt";

            // 저장할 위치의 초기 디렉토리를 설정합니다.
            // Environment.CurrentDirectory: 현재 디렉토리를 나타냅니다.
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Text = saveFileDialog1.FileName;

                string result = "";
                for (int i = 0; i < Nov.Count(); i++) 
                {
                    result += Nov[i][1]+"\n\r";
                }
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    sw.Write(result);
                }
            }
        }
    }
}
