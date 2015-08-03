using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MebiusLib
{
    ///make mini class that has Key and Val property
    ///usage : make viewmodel has [HogeList, SelectedHogeStr]
    ///init HogeList and SelectedHogeStr in constructor of viewmodel
    ///HogeList is list or array with mini class
    ///xaml : DisplayMemberPath="Key" SelectedValuePath="Val" SelectedValue="{Binding SelectedHogeStr}"
    ///ItemsSource="{Binding HogeList}"
    ///Command="{x:Static AppNameSpace:HogeCommands.HogeCmd}"
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
