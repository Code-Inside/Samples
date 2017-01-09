using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IdentityModel.OidcClient.WebView;

namespace IdentityTest.WpfClient.OidcStuff
{
    public class WinFormsWebView : IWebView
    {
        private readonly Func<Form> _formFactory;

        public WinFormsWebView(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

        public WinFormsWebView(string title = "Authenticating ...", int width = 1024, int height = 768)
            : this(() => new Form
            {
                Name = "WebAuthentication",
                Text = title,
                Width = width,
                Height = height
            })
        { }

        public event EventHandler<HiddenModeFailedEventArgs> HiddenModeFailed;

        public async Task<InvokeResult> InvokeAsync(InvokeOptions options)
        {
            using (var form = _formFactory.Invoke())
            using (var browser = new ExtendedWebBrowser()
            {
                Dock = DockStyle.Fill
            })
            {
                var signal = new SemaphoreSlim(0, 1);

                var result = new InvokeResult
                {
                    ResultType = InvokeResultType.UserCancel
                };

                form.FormClosed += (o, e) =>
                {
                    signal.Release();
                };

                browser.NavigateError += (o, e) =>
                {
                    e.Cancel = true;
                    result.ResultType = InvokeResultType.HttpError;
                    result.Error = e.StatusCode.ToString();
                    signal.Release();
                };

                browser.BeforeNavigate2 += (o, e) =>
                {
                    if (e.Url.StartsWith(options.EndUrl))
                    {
                        e.Cancel = true;
                        result.ResultType = InvokeResultType.Success;
                        if (options.ResponseMode == ResponseMode.FormPost)
                        {
                            result.Response = Encoding.UTF8.GetString(e.PostData ?? new byte[] { });
                        }
                        else
                        {
                            result.Response = e.Url;
                        }
                        signal.Release();
                    }
                };

                form.Controls.Add(browser);
                browser.Show();

                System.Threading.Timer timer = null;
                if (options.InitialDisplayMode != DisplayMode.Visible)
                {
                    result.ResultType = InvokeResultType.Timeout;
                    timer = new System.Threading.Timer((o) =>
                    {
                        var args = new HiddenModeFailedEventArgs(result);
                        HiddenModeFailed?.Invoke(this, args);
                        if (args.Cancel)
                        {
                            browser.Stop();
                            form.Invoke(new Action(() => form.Close()));
                        }
                        else
                        {
                            form.Invoke(new Action(() => form.Show()));
                        }
                    }, null, (int)options.InvisibleModeTimeout.TotalSeconds * 1000, Timeout.Infinite);
                }
                else
                {
                    form.Show();
                }

                browser.Navigate(options.StartUrl);

                await signal.WaitAsync();
                if (timer != null) timer.Change(Timeout.Infinite, Timeout.Infinite);

                form.Hide();
                browser.Hide();

                return result;
            }
        }
    }
}