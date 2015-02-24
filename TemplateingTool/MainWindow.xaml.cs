using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using DotLiquid;
using ICSharpCode.AvalonEdit.Highlighting;

namespace TemplateingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();

            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("TemplateingTool.AwlLiquidHighlighting.xshd"))
            {
                using (XmlReader reader = new XmlTextReader(s))
                {
                    templateEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        private async void templateEditor_TextChanged(object sender, EventArgs e)
        {
            parseTemplate();
        }

        private async void txtObjectList_TextChanged(object sender, TextChangedEventArgs e)
        {
            parseTemplate();
        }

        private void chkObjectsAsList_Checked(object sender, RoutedEventArgs e)
        {
            parseTemplate();
        }

        private void chkObjectsAsList_Unchecked(object sender, RoutedEventArgs e)
        {
            parseTemplate();
        }

        private Task parseTask;
        private async void parseTemplate()
        {
            try
            {
                var objects = txtObjectList.Text.Replace("\r\n", "\n").Split('\n');
                var template = templateEditor.Text;
                var objAsList = chkObjectsAsList.IsChecked.Value;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var t = DotLiquid.Template.Parse(template);

                        var g = new StringBuilder();

                        if (!objAsList)
                        {
                            foreach (var obj in objects)
                            {
                                var result = t.Render(Hash.FromAnonymousObject(new
                                {
                                    @object = obj
                                }));

                                g.Append(result);
                            }
                        }
                        else
                        {
                            var result = t.Render(Hash.FromAnonymousObject(new
                            {
                                objects
                            }));

                            g.Append(result);
                        }


                        var res = g.ToString();
                        Dispatcher.BeginInvoke(new Action(() => resultEditor.Text = res));
                    }
                    catch (Exception ex)
                    { }
                });
            }
            catch (Exception ex)
            { }
        }
    }
}
