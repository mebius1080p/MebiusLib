using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PictlogicaHelper
{
    class CommandCombobox : ComboBox, ICommandSource
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(CommandCombobox),
            new PropertyMetadata((ICommand)null,
            new PropertyChangedCallback(CommandCombobox.CommandChanged)));
        public int ID { get; set; }
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return new object(); }
        }

        public IInputElement CommandTarget
        {
            get { return this; }
        }
        public CommandCombobox() : base()
        {
            //do nothing
        }
        private static void CommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            //do nothing
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {//オーバーライド！
            base.OnSelectionChanged(e);
            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;
                if (command != null)
                {
                    command.Execute(this.CommandParameter, this.CommandTarget);
                }
                else
                {
                    ((ICommand)this.Command).Execute(this.CommandParameter);
                }
            }
        }
    }
}
