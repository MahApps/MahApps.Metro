using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;

namespace MetroDemo.ExampleViews
{
    public class Person : DependencyObject
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for PersonInputDialog.xaml
    /// </summary>
    public partial class PersonInputDialog : CustomInputDialog<Person> {
        public PersonInputDialog() {
            People = new List<Person>() {
                new Person() {Age = 22,Name = "Dave"},
                new Person() {Age = 34,Name = "John"},
                new Person() {Age = 55,Name = "Ian"}
            };
            SelectedPerson = People[0];

            InitializeComponent();
        }   

        public override Task<Person> WaitForButtonPressAsync() {

            Dispatcher.BeginInvoke(new Action(() => {
                this.Focus();
            }));

            var tcs = new TaskCompletionSource<Person>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() => {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                this.KeyDown -= escapeKeyHandler;

                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;

                cancellationTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) => {
                if (e.Key == Key.Escape) {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            negativeKeyHandler = (sender, e) => {
                if (e.Key == Key.Enter) {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            affirmativeKeyHandler = (sender, e) => {
                if (e.Key == Key.Enter) {
                    cleanUpHandlers();

                    tcs.TrySetResult(SelectedPerson);
                }
            };

            negativeHandler = (sender, e) => {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) => {
                cleanUpHandlers();

                tcs.TrySetResult(SelectedPerson);

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        public List<Person> People { get; set; }

        public Person SelectedPerson { get; set; }

    }
}
