using Avalonia.Controls;
using eStationDemo.ViewModel;

namespace eStationDemo.View;

public partial class PublishESLTask : Window
{
    public PublishESLTask(string clientId)
    {
        InitializeComponent();

        ((PublishESLTaskViewModel)DataContext).SetClientID(clientId);
    }
}