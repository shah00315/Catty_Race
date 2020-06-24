using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Catty_Race
{
    public partial class Form1 : Form
    {
        private List<RadioButton> radioButtons = new List<RadioButton>();
        private Racing world = new Racing();

        // two things to control state of the form.
        private Punter selected;

        private Action Max;

        public Form1()
        {
            InitializeComponent();


            // this Monkey patches the radio buttons to work outside the usual group box
            radioButtons.Add(rdoJoe);
            radioButtons.Add(rdoBob);
            radioButtons.Add(rdoAlice);
            foreach (var radio in radioButtons)
            {
                radio.CheckedChanged -= PerformMutualExclusion;
                radio.CheckedChanged += PerformMutualExclusion;
            }

            gboxJoe.Text = world.Joe.name;
            gboxBob.Text = world.Bob.name;
            gboxAlice.Text = world.Alice.name;

            rdoJoe.CheckedChanged += setNumAmount(world.Joe);
            rdoBob.CheckedChanged += setNumAmount(world.Bob);
            rdoAlice.CheckedChanged += setNumAmount(world.Alice);
        }

        private void PerformMutualExclusion(object sender, EventArgs e)
        {
            var senderRadio = (RadioButton)sender;
            if (!senderRadio.Checked)
            {
                return;
            }

            foreach (var radio in radioButtons)
            {
                if (radio == sender || !radio.Checked)
                {
                    continue;
                }
                radio.Checked = false;
            }
        }

        //setting the maxium amount to bet.
        private EventHandler setNumAmount(Punter punter)
        {
            return (delegate (object sender, EventArgs e)
            {
                var senderRadio = (RadioButton)sender;
                if (!senderRadio.Checked)
                {
                    return;
                }

                Max = Maximize(punter);
                Max();
            });
        }


        private Action Maximize(Punter punter)
        {
            return delegate ()
            {
                nuAmount.Maximum = (decimal)punter.Cash;
                selected = punter;
            };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            new Task(() => animate(world.PinkHound, picboxPink, 0.0)).RunSynchronously();
            new Task(() => animate(world.BlueHound, picboxBlue, 300.0)).RunSynchronously();
            new Task(() => animate(world.TealHound, picboxTeal, 600.0)).RunSynchronously();
            Draw();
        }

        private async void animate(Greyhound hound, PictureBox pb, double seed)
        {
            var ix = pb.Location.X;
            var iy = pb.Location.Y;

            var finishLine = FinishLine.Location.X - ix;
            double div = finishLine / 1000.0;


            var w = pb.Width;
            var h = pb.Height;
            double loop = seed;
            while (true)
            {
                int hx = (int)(div * (double)hound.position);
                int x = (int)Math.Round(5.0 * Math.Sin(10 * loop / 1000.0));
                int y = (int)Math.Round(5.0 * Math.Cos(10 * loop / 1000.0));
                pb.Location = new Point(x + ix + hx, (2 * y) + iy);
                pb.Width = w + (-x * 2);
                pb.Height = h + (-y * 2);
                await Task.Delay(16);
                loop += 16.0;
            }
        }

        private void Draw()
        {
            DrawPunter(world.Alice, lblAliceWallet, lblAliceBetAmount, lblAliceBetOn);
            DrawPunter(world.Joe, lblJoeWallet, lblJoeBetAmount, lblJoeBetOn);
            DrawPunter(world.Bob, lblBobWallet, lblBobBetAmount, lblBobBetOn);
        }

        private void DrawPunter(Punter punter, Label wallet, Label amount, Label Cat)
        {
            amount.Text = punter.betAmount.ToString();
            wallet.Text = punter.Cash.ToString();
            try
            {
                Cat.Text = punter.betHound.name;
            }
            catch (NullReferenceException)
            {
                Cat.Text = "none";
            }
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            HoundEnum hound = HoundEnum.Pink;

            switch (cbxHound.SelectedIndex)
            {
                case 0:
                    hound = HoundEnum.Pink;
                    break;
                case 1:
                    hound = HoundEnum.Teal;
                    break;
                case 2:
                    hound = HoundEnum.Blue;
                    break;

            }

            world.Bet(hound, selected, (int)nuAmount.Value);
            Draw();

        }

        // we check  for bankruptancy and blank the gboxes
        private void Bankrupt()
        {
            foreach (Punter punter in world.Punters)
            {
                if (punter.Cash == 0)
                {
                    GroupBox gbox = null;
                    if (punter == world.Alice) gbox = gboxAlice;
                    if (punter == world.Bob) gbox = gboxBob;
                    if (punter == world.Joe) gbox = gboxJoe;

                    gbox.Enabled = false;
                }
            }
        }

        private void GameOver()
        {
            bool allBankrupt = true;
            foreach (Punter punter in world.Punters)
            {
                allBankrupt = punter.Cash == 0 && allBankrupt;
            }

            if (allBankrupt)
            {
                world = new Racing();
                MessageBox.Show("GAME OVER!");
                gboxJoe.Enabled = true;
                gboxAlice.Enabled = true;
                gboxBob.Enabled = true;
                Draw();
            }

        }

        private async void btnRace_Click(object sender, EventArgs e)
        {
            var winner = await world.Race();

            Draw();
            MessageBox.Show(winner.name + " has one!");
            world.Reset();
            Bankrupt();
            Max();
            GameOver();
        }
    }
}
