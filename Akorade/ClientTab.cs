using Doumi.Nexon.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akorade
{
    public partial class ClientTab : TabPage
    {
        public Client Client { get; set; }
        public Server server { get; set; }

        private ImageList imlSkills;
        private ImageList imlSpells;

        public ClientTab(Client client)
        {
            InitializeComponent();
            Client = client;
            server = Program.Servers[0];
        }

        private void BtnLogClient_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogServer.Checked)
            {
                Client.Client.OnDataSending += new Action<NexonPacket>(Client_OnDataSending);
            }
            else
            {
                Client.Client.OnDataSending -= new Action<NexonPacket>(Client_OnDataSending);
            }
        }

        private void Client_OnDataSending(NexonPacket packet)
        {
            this.ThreadSafeInvoke(delegate
            {
                if (textConsoleOutput.Text.Length > 0)
                {
                    textConsoleOutput.SynchronizedInvoke(() => textConsoleOutput.AppendText(Environment.NewLine));
                }
                textConsoleOutput.SynchronizedInvoke(() => textConsoleOutput.AppendText("Recv: " + packet.ToString()));
                textConsoleOutput.ScrollToCaret();
            });
        }

        private void ChkLogClient_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogClient.Checked)
            {
                Client.Server.OnDataSending += new Action<NexonPacket>(Server_OnDataSending);
            }
            else
            {
                Client.Server.OnDataSending -= new Action<NexonPacket>(Server_OnDataSending);
            }
        }

        private void Server_OnDataSending(NexonPacket packet)
        {
            this.ThreadSafeInvoke(delegate
            {
                if (textConsoleOutput.Text.Length > 0)
                {
                    textConsoleOutput.SynchronizedInvoke(() => textConsoleOutput.AppendText(Environment.NewLine));
                }
                textConsoleOutput.SynchronizedInvoke(() => textConsoleOutput.AppendText("Send: " + packet.ToString()));
                textConsoleOutput.ScrollToCaret();
            });
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            this.ThreadSafeInvoke(delegate
            {
                textConsoleOutput.Clear();
            });
        }

        private void LangSelector_TextChanged(object sender, EventArgs e)
        {
            Client.langSelect = langSelector.Text;
        }

        private async void BtnsubmitNPCTextEdit_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lblkoreanText.Text, txtenglishNPCText.Text);
        }

        private async void BtnNPCNameEdit_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lblnpcName.Text, txtnpcName.Text);
        }

        private async void BtnSubmitOptionEdit_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lstKoreanNPCOptions.GetItemText(lstKoreanNPCOptions.SelectedItem), txtPursuitText.Text);
            txtPursuitText.Clear();
        }

        private void LstKoreanNPCOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstnpcOptions.SelectedIndex = lstKoreanNPCOptions.SelectedIndex;
        }

        private void LstnpcOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstKoreanNPCOptions.SelectedIndex = lstnpcOptions.SelectedIndex;
            txtPursuitText.Text = lstnpcOptions.GetItemText(lstnpcOptions.SelectedItem);
        }

        private async void BtnSubmitMapEdit_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lblMapName.Text, txtMapName.Text);
            txtMapName.Clear();
        }

        public void LoadSkills()
        {
            for (int i = 0; i < Client.Skillbook.Length; i++)
            {
                if (Client.Skillbook[i] != null)
                    lstKoreanSkill.Items.Add(Client.Skillbook[i].Text);
            }
            for (int i = 0; i < Client.Skillbook.Length; i++)
            {
                if (Client.Skillbook[i] != null)
                    lstEnglishSkill.Items.Add(Client.Skillbook[i].EnglishName);
            }
            for (int i = 0; i < Client.Spellbook.Length; i++)
            {
                if (Client.Spellbook[i] != null)
                    lstKoreanSpell.Items.Add(Client.Spellbook[i].Text);
            }
            for (int i = 0; i < Client.Spellbook.Length; i++)
            {
                if (Client.Spellbook[i] != null)
                    lstEngSpell.Items.Add(Client.Spellbook[i].EnglishName);
            }
        }

        private async void Button3_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lstKoreanSkill.GetItemText(lstKoreanSkill.SelectedItem), txtSkillName.Text);
            txtSkillName.Clear();
        }

        private async void BtnSpellEdit_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lstKoreanSpell.GetItemText(lstKoreanSpell.SelectedItem), txtSpellName.Text);
            txtSkillName.Clear();
        }

        private void LstEnglishSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstKoreanSkill.SelectedIndex = lstEnglishSkill.SelectedIndex;
            txtSkillName.Text = lstEnglishSkill.GetItemText(lstEnglishSkill.SelectedItem);
        }

        private void LstKoreanSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstEnglishSkill.SelectedIndex = lstKoreanSkill.SelectedIndex;
        }

        private void LstKoreanSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstKoreanSpell.SelectedIndex = lstEngSpell.SelectedIndex;
        }

        private void LstEngSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstKoreanSpell.SelectedIndex = lstEngSpell.SelectedIndex;
            txtSpellName.Text = lstEngSpell.GetItemText(lstEngSpell.SelectedItem);
        }

        private void LstKoreanStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstEnglishStatus.SelectedIndex = lstKoreanStatus.SelectedIndex;
        }

        private void LstEnglishStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstKoreanStatus.SelectedIndex = lstEnglishStatus.SelectedIndex;
            txtEnglishStatus.Text = lstEnglishStatus.GetItemText(lstEnglishStatus.SelectedItem);
        }

        private async void BtnSubmitStatus_Click(object sender, EventArgs e)
        {
            await server.requestTranslationEdit(lstKoreanStatus.GetItemText(lstKoreanStatus.SelectedItem), txtEnglishStatus.Text);
            txtEnglishStatus.Clear();
        }

        private void BtnClearStatus_Click(object sender, EventArgs e)
        {
            lstKoreanStatus.Items.Clear();
            lstEnglishStatus.Items.Clear();
            txtEnglishStatus.Clear();
        }

        private void ToolStripCopy_Click(object sender, EventArgs e)
        {
            string UserInfo = lblnpcName.Text + " - " + txtnpcName.Text + Environment.NewLine + lblkoreanText.Text + " - " + txtenglishNPCText.Text + Environment.NewLine + lblMapName.Text + " - " + txtMapName.Text + Environment.NewLine;
            string[] KoroptionList = lstKoreanNPCOptions.Items.OfType<string>().ToArray();
            string[] EngoptionList = lstnpcOptions.Items.OfType<string>().ToArray();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < KoroptionList.Length; i++)
            {
                builder.Append(KoroptionList[i] + " - " + EngoptionList[i]).AppendLine();
            }
            Clipboard.SetText(UserInfo + builder.ToString());
        }
    }
}
