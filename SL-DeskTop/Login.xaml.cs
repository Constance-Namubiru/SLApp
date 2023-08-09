using Data;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace SL_DeskTop
{
    /// <summary>
    /// Interaction logic for Lab.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly HttpClient _httpClient;
        private Lab lab;
        public Login()
        {
            InitializeComponent();


            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("https://localhost:7194/");
            lab = new Lab();

           
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = new User() { Email = txtemail.Text, Password = txtpwd.Password };

            var postContent = new StringContent(JsonSerializer.Serialize(user, new JsonSerializerOptions()
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json");




            var result = _httpClient.PostAsync("Accounts", postContent).GetAwaiter().GetResult();

            if (result.IsSuccessStatusCode)
            {
                var message = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var exists = JsonSerializer.Deserialize<bool>(message);

                switch (exists)
                {
                    case true:
                        lblNotice.Content = message;
                        Hide();
                        lab.Show();
                        break;

                    case false:

                        lblNotice.Content = "Invalid user name or password";

                        break;

                }

            }

            else
            {
                lblNotice.Content = result.ReasonPhrase;
            }



        }
    }
}
