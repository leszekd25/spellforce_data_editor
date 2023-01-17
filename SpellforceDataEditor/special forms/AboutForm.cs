using System.Drawing;
using System.Windows.Forms;

namespace SpellforceDataEditor.special_forms
{
    public partial class AboutForm : Form
    {
        LinkLabel link_wiki;
        LinkLabel link_discord;
        LinkLabel link_nexus;

        public AboutForm()
        {
            InitializeComponent();

            link_wiki = new LinkLabel();
            link_wiki.Text = "SpellForce Editor manual";
            link_wiki.LinkClicked += new LinkLabelLinkClickedEventHandler(link_wiki_Clicked);
            LinkLabel.Link data = new LinkLabel.Link();
            data.LinkData = "https://drive.google.com/file/d/1GO6NQdl-1x7wgddGORdIAhZj86SWGmum/view?usp=sharing";
            link_wiki.Links.Add(data);
            link_wiki.AutoSize = true;
            link_wiki.Padding = new Padding(0);

            link_discord = new LinkLabel();
            link_discord.Text = "SpellForce Community Discord";
            link_discord.LinkClicked += new LinkLabelLinkClickedEventHandler(link_discord_Clicked);
            data = new LinkLabel.Link();
            data.LinkData = "https://discordapp.com/invite/spellforce";
            link_discord.Links.Add(data);
            link_discord.AutoSize = true;

            link_nexus = new LinkLabel();
            link_nexus.Text = "my profile";
            link_nexus.LinkClicked += new LinkLabelLinkClickedEventHandler(link_nexus_Clicked);
            data = new LinkLabel.Link();
            data.LinkData = "https://forums.nexusmods.com/index.php?/user/53072901-leszekd25/";
            link_nexus.Links.Add(data);
            link_nexus.AutoSize = true;

            TextBoxAbout.DeselectAll();

            TextBoxAbout.SelectionAlignment = HorizontalAlignment.Left;
            TextBoxAbout.SelectionFont = new Font("Arial", 12, FontStyle.Bold);
            TextBoxAbout.AppendText("Spellforce Editor");

            TextBoxAbout.SelectionAlignment = HorizontalAlignment.Left;
            TextBoxAbout.SelectionFont = new Font("Arial", 8, FontStyle.Regular);
            TextBoxAbout.AppendText("\r\n\r\n...is a set of tools which allow you to modify SpellForce to your liking. It includes gamedata editor, map editor, and much more.\r\n" +
                                    "Originally created by Insigar from spellforcefanforum, completely redone and currently maintained by me, creator of this tool.");

            TextBoxContactInfo.SelectionAlignment = HorizontalAlignment.Left;
            TextBoxContactInfo.SelectionFont = new Font("Arial", 8, FontStyle.Regular);
            TextBoxContactInfo.AppendText("Links of semi-importance:\r\n");

            link_wiki.Location =
                TextBoxContactInfo.GetPositionFromCharIndex(TextBoxContactInfo.TextLength);
            TextBoxContactInfo.Controls.Add(link_wiki);
            TextBoxContactInfo.AppendText(link_wiki.Text + "  , an up-to-date manual for most of this program's caapabilities\r\n");
            TextBoxContactInfo.SelectionStart = TextBoxContactInfo.TextLength;

            link_discord.Location =
                TextBoxContactInfo.GetPositionFromCharIndex(TextBoxContactInfo.TextLength);
            TextBoxContactInfo.Controls.Add(link_discord);
            TextBoxContactInfo.AppendText(link_discord.Text + "  , where you can find a SF1 modding community among other things\r\n");
            TextBoxContactInfo.SelectionStart = TextBoxContactInfo.TextLength;

            TextBoxContactInfo.AppendText("\r\nIn case you find a bug or want some feature added/improved upon,\r\ncontact me:\r\non Discord: shovel_knight#7698\r\non NexusMods: ");
            link_nexus.Location =
                TextBoxContactInfo.GetPositionFromCharIndex(TextBoxContactInfo.TextLength);
            TextBoxContactInfo.Controls.Add(link_nexus);
            TextBoxContactInfo.AppendText(link_nexus.Text);

        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            link_wiki.LinkClicked -= new LinkLabelLinkClickedEventHandler(link_wiki_Clicked);
            link_discord.LinkClicked -= new LinkLabelLinkClickedEventHandler(link_discord_Clicked);
            link_nexus.LinkClicked -= new LinkLabelLinkClickedEventHandler(link_nexus_Clicked);
        }

        private void link_wiki_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void link_discord_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void link_nexus_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
