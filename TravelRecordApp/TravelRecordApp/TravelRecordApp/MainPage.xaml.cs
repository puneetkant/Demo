using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TravelRecordApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            bool isEmptyPassword = string.IsNullOrEmpty(passwordEntry.Text);
            bool isEmptyEmail = string.IsNullOrEmpty(emailEntry.Text);

            if (isEmptyEmail || isEmptyPassword)
            {

            }
            else
            {
                Navigation.PushAsync(new HomePage());
            }
        }
    }
}
