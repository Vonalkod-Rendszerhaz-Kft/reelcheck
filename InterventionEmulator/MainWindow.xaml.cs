using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using ReelCheck.EventHub.Contracts;
using Vrh.PrintingService.EventHubContract;
using Vrh.CameraService.EventHubContract;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace InterventionEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Interventions

        private void BlockingButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.Blocking.Block();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.Blocking.Block, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"Blocking ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void UnBlockingButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.Blocking.UnBlock();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.Blocking.UnBlock, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"UnBlocking ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.Activate()
            {
                UserName = UserNameTextBox.Text,
            };
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.Activate, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"Login ok {rq.UserName}");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.InActivate()
            {
                UserName = UserNameTextBox.Text,
            };
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.InActivate, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"Logout ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void RePrintButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.RePrint();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.RePrint, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"RePrint ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void SkipPrintButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.SkipPrint();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.SkipPrint, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"SkipPrint ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void LabeldRadio_Checked(object sender, RoutedEventArgs e)
        {
            LabelDEventHubChannelNameTextbox.Text = "LABELD";
        }

        private void LabelERadio_Checked(object sender, RoutedEventArgs e)
        {
            LabelDEventHubChannelNameTextbox.Text = "LABELE";
        }

        private void GSDueDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.GoldenSampleMode.DeleteDue();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.DeleteDue, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq, new TimeSpan(0, 0, 5));
                LogListView.Items.Insert(0, $"GS Due Delete ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        #region NOT USED

        private void ReIdentifyButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.ReIdentify();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.ReIdentify, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"ReIdentify ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void ReCheckButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.ReCheck();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.ReCheck, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"ReCheck ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void GoldenSampleModeEnterButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.GoldenSampleMode.Enter();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.Enter, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"Golden SampleMode Enter ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void GoldenSampleModeExitButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.GoldenSampleMode.Exit();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.Exit, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"Golden SampleMode Exit ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void GoldenSampleModeExtendValidityButton_Click(object sender, RoutedEventArgs e)
        {
            //var rq = new InterventionContract.GoldenSampleMode.ExtendValidity()
            //{
            //    Qty = int.Parse(QtyTextBox.Text),
            //};
            //try
            //{
            //    EventHubCore.Call<RedisPubSubChannel, InterventionContract.GoldenSampleMode.ExtendValidity, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
            //    LogListView.Items.Insert(0, $"Golden SampleMode ExtendValidity ok {rq.Qty}");
            //}
            //catch (Exception ex)
            //{
            //    LogListView.Items.Insert(0, ex.Message);
            //}
        }
                
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UnloadModeEnterButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.UnloadMode.Enter();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.UnloadMode.Enter, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"UnloadMode Enter ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void UnloadModeExitButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.UnloadMode.Exit();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.UnloadMode.Exit, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"UnloadMode Exit ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void ManualModeEnterButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.ManualMode.On();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.ManualMode.On, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"ManualMode Enter ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void ManualModeExitButton_Click(object sender, RoutedEventArgs e)
        {
            var rq = new InterventionContract.ManualMode.Off();
            try
            {
                EventHubCore.Call<RedisPubSubChannel, InterventionContract.ManualMode.Off, InterventionContract.Result>(LabelDEventHubChannelNameTextbox.Text, rq);
                LogListView.Items.Insert(0, $"ManualMode Exit ok");
            }
            catch (Exception ex)
            {
                LogListView.Items.Insert(0, ex.Message);
            }
        }

        private void ManualModeDataButton_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion NOT USED

        #endregion Interventions

        #region Printing

        private void PrintingEventHubSubscribe()
        {
            if (PrinterEmulatorActive.IsChecked.Value)
            {
                try
                {
                    EventHubCore.RegisterHandler<RedisPubSubChannel, PrintMessage, MessageResult>(PrintingEventHubChannelNameTextBox.Text, PrintingEmulation);
                    PrintingLogListView.Items.Insert(0, $"Printing emulation started on {PrintingEventHubChannelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    PrintingLogListView.Items.Insert(0, ex.Message);
                }
            }
            else
            {
                try
                {
                    EventHubCore.DropChannel<RedisPubSubChannel>(PrintingEventHubChannelNameTextBox.Text);
                    PrintingLogListView.Items.Insert(0, $"Printing emulation stoped {PrintingEventHubChannelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    PrintingLogListView.Items.Insert(0, ex.Message);
                }
            }
        }

        private string printingState = "Success Printing";

        public Response<MessageResult> PrintingEmulation(Request<PrintMessage, MessageResult> message)
        {
            Response<MessageResult> Rmr = message.MyResponse;
            try
            {
                if (printingState != "Success Printing")
                {
                    throw new Exception(printingState);
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    PrintingLogListView.Items.Insert(0, $"Success Printing");
                }));
                //PrintingLogListView.Items.Insert(0, $"Success Printing");
                message.RequestContent.SendingState = true;
                Rmr.ResponseContent = new MessageResult();
            }
            catch (Exception ex)
            {
                Rmr.Exception = ex;
                Dispatcher.Invoke(new Action(() =>
                {
                    PrintingLogListView.Items.Insert(0, $"{ex.Message}");
                }));                
            }

            return Rmr;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            if (ck.IsChecked.Value)
            {
                printingState = ck.Content.ToString();
            }
        }

        private void PrinterEmulatorActive_Click(object sender, RoutedEventArgs e)
        {
            PrintingEventHubSubscribe();
        }

        #endregion Printing

        #region IdCamera

        #region LabelD

        private void LabelDIdCameraActive_Click(object sender, RoutedEventArgs e)
        {
            LabelDIdCameraEventHubSubscribe();
        }

        private void LabelDIdCameraEventHubSubscribe()
        {
            if (LabelDIdCameraActive.IsChecked.Value)
            {
                try
                {
                    EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(LabelDIdCameraChennelNameTextBox.Text, LabelDSetIO);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, GetIOMessage, IOChangeMessageResult>(LabelDIdCameraChennelNameTextBox.Text, LabelDGetIO);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(LabelDIdCameraChennelNameTextBox.Text, LabelDRead);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(LabelDIdCameraChennelNameTextBox.Text, LabelDSetIOExt);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, GetIOEXTIOMessage, IOEXTIOStateMessageResult>(LabelDIdCameraChennelNameTextBox.Text, LabelDGetIOExt);
                    LabelDIdCameraAppendLog($"ID camera emulation started on {LabelDIdCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelDIdCameraAppendLog(ex.Message);
                }
            }
            else
            {
                try
                {
                    EventHubCore.DropChannel<RedisPubSubChannel>(LabelDIdCameraChennelNameTextBox.Text);
                    LabelDIdCameraAppendLog($"ID camera emulation stoped on {LabelDIdCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelDIdCameraAppendLog(ex.Message);
                }
            }
        }

        private Response<IOEXTIOStateMessageResult> LabelDGetIOExt(Request<GetIOEXTIOMessage, IOEXTIOStateMessageResult> request)
        {
            var myResponse = request.MyResponse;
            bool ioValue = IdGrid.Dispatcher.Invoke<bool>(() => { return LabelDGetIOExtValue(request.RequestContent.IOPort); }, DispatcherPriority.Normal);
            myResponse.ResponseContent = new IOEXTIOStateMessageResult(request.RequestContent.IOPort, ioValue);
            return myResponse;
        }

        private Response<CommonMessageResult> LabelDSetIOExt(Request<SetIOEXTIOMessage, CommonMessageResult> request)
        {
            var myResponse = request.MyResponse;
            IdGrid.Dispatcher.BeginInvoke(new Action<byte, bool>(LabelDSetIOExtValue)
                                   , DispatcherPriority.Background, request.RequestContent.IOEXTPort, request.RequestContent.TurneOn);
            LabelDIdCameraLog($"SET IO receive ({request.RequestContent.IOEXTPort} to {request.RequestContent.TurneOn}) request id {request.Id}");
            return myResponse;
        }

        private void LabelDSetIOExtValue(byte ioNumber, bool value)
        {
            switch (ioNumber)
            {
                default:
                    break;
                case 0:
                    LabelDAllStationFinished.IsChecked = value;
                    break;
                case 1:
                    LabelDWorkpieceOk.IsChecked = value;
                    break;
                case 2:
                    LabelDWorkpieceNok.IsChecked = value;
                    break;
                case 3:
                    LabelDEmpty.IsChecked = value;
                    break;
            }
        }

        private bool LabelDGetIOExtValue(byte ioNumber)
        {
            switch (ioNumber)
            {
                default:
                    return false;
                case 0:
                    return (bool)LabelDStickingDone.IsChecked;
                case 1:
                    return (bool)LabelDStatusReset.IsChecked;
                case 2:
                    return (bool)LabelDHardReset.IsChecked;
            }
        }

        private Response<LabelDataMessageResult> LabelDRead(Request<ReadMessage, LabelDataMessageResult> request)
        {
            string responseContent = LabelDReadResponseComboBox.Dispatcher.Invoke(LabelDGetReadResponseComboText, DispatcherPriority.Normal);
            LabelDIdCameraLog($"Read receive, response with: {responseContent}");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new LabelDataMessageResult(responseContent);
            return myResponse;
        }

        private string LabelDGetReadResponseComboText()
        {
            return LabelDReadResponseComboBox.Text;
        }

        private Response<CommonMessageResult> LabelDSetIO(Request<SetIOMessage, CommonMessageResult> request)
        {
            var myResponse = request.MyResponse;
            switch (request.RequestContent.Bulb)
            {
                case Bulbs.Green:
                    LabelDReady.Dispatcher.BeginInvoke(new Action<bool>(LabelDSetReady)
                                           , DispatcherPriority.Send, request.RequestContent.BulbTurneOn);
                    break;
                case Bulbs.Red:
                    LabelDLabelPrinted.Dispatcher.BeginInvoke(new Action<bool>(LabelDSetLebelPrinted)
                                           , DispatcherPriority.Send, request.RequestContent.BulbTurneOn);
                    break;
                default:
                    break;
            }
            LabelDIdCameraLog($"SET IO receive ({request.RequestContent.Bulb} to {request.RequestContent.BulbTurneOn}) request id {request.Id}");
            return myResponse;
        }

        private Response<IOChangeMessageResult> LabelDGetIO(Request<GetIOMessage, IOChangeMessageResult> request)
        {
            LabelDIdCameraLog($"GET IO receive");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new IOChangeMessageResult(IdGrid.Dispatcher.Invoke<bool>(() => { return LabelDEnable.IsChecked.Value; }, DispatcherPriority.Normal));
            return myResponse;
        }

        private void LabelDSetReady(bool value)
        {
            LabelDReady.IsChecked = value;
        }

        private void LabelDSetLebelPrinted(bool value)
        {
            LabelDLabelPrinted.IsChecked = value;
        }

        private void LabelDEnable_Click(object sender, RoutedEventArgs e)
        {
            var message = new IOChangeMessageResult(LabelDEnable.IsChecked.Value);
            EventHubCore.SendAsync<RedisPubSubChannel, IOChangeMessageResult>(LabelDIdCameraChennelNameTextBox.Text, message);
            LabelDIdCameraLog($"Enable to {LabelDEnable.IsChecked.Value} IOCAHNGE SEND");
        }

        private void LabelDStickingDone_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(0, LabelDStickingDone.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelDIdCameraChennelNameTextBox.Text, msg);
            LabelDIdCameraLog($"StickingDone to {LabelDStickingDone.IsChecked.Value} IOCAHNGE SEND");
        }

        private void LabelDStatusReset_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(1, LabelDStatusReset.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelDIdCameraChennelNameTextBox.Text, msg);
            LabelDIdCameraLog($"StatusReset to {LabelDStatusReset.IsChecked.Value} IOCAHNGE SEND");
        }

        private void LabelDHardReset_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(2, LabelDHardReset.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelDIdCameraChennelNameTextBox.Text, msg);
            LabelDIdCameraLog($"HardReset to {LabelDHardReset.IsChecked.Value} IOCAHNGE SEND");
        }

        #region Log

        private void LabelDIdCameraLog(string line)
        {
            LabelDIdCameraLogListView.Dispatcher.BeginInvoke(new Action<string>(LabelDIdCameraAppendLog)
                                   , DispatcherPriority.Send, line);
        }

        private void LabelDIdCameraAppendLog(string line)
        {
            LabelDIdCameraLogListView.Items.Add(line);
        }

        private void LabelDClearIdCameraLogButton_Click(object sender, RoutedEventArgs e)
        {
            LabelDIdCameraLogListView.Items.Clear();
        }

        #endregion Log

        #endregion LabelD

        #region LabelE

        private void LabelEIdCameraActive_Click(object sender, RoutedEventArgs e)
        {
            LabelEIdCameraEventHubSubscribe();
        }

        private void LabelEIdCameraEventHubSubscribe()
        {
            if (LabelEIdCameraActive.IsChecked.Value)
            {
                try
                {
                    EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOMessage, CommonMessageResult>(LabelEIdCameraChennelNameTextBox.Text, LabelESetIO);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, GetIOMessage, IOChangeMessageResult>(LabelEIdCameraChennelNameTextBox.Text, LabelEGetIO);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(LabelEIdCameraChennelNameTextBox.Text, LabelERead);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>(LabelEIdCameraChennelNameTextBox.Text, LabelESetIOExt);
                    EventHubCore.RegisterHandler<RedisPubSubChannel, GetIOEXTIOMessage, IOEXTIOStateMessageResult>(LabelEIdCameraChennelNameTextBox.Text, LabelEGetIOExt);
                    LabelEIdCameraAppendLog($"ID camera emulation started on {LabelEIdCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelEIdCameraAppendLog(ex.Message);
                }
            }
            else
            {
                try
                {
                    EventHubCore.DropChannel<RedisPubSubChannel>(LabelEIdCameraChennelNameTextBox.Text);
                    LabelEIdCameraAppendLog($"ID camera emulation stoped on {LabelEIdCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelEIdCameraAppendLog(ex.Message);
                }
            }
        }

        private Response<IOEXTIOStateMessageResult> LabelEGetIOExt(Request<GetIOEXTIOMessage, IOEXTIOStateMessageResult> request)
        {
            var myResponse = request.MyResponse;
            bool ioValue = IdGrid.Dispatcher.Invoke<bool>(() => { return LabelEGetIOExtValue(request.RequestContent.IOPort); }, DispatcherPriority.Normal);
            myResponse.ResponseContent = new IOEXTIOStateMessageResult(request.RequestContent.IOPort, ioValue);
            return myResponse;
        }

        private bool LabelEGetIOExtValue(byte ioNumber)
        {
            switch (ioNumber)
            {
                default:
                    return false;
                case 0:
                    return (bool)LabelEStickingDone.IsChecked;
                case 1:
                    return (bool)LabelECheckCameraEnable.IsChecked;
                case 2:
                    return (bool)LabelEEnable.IsChecked;
            }
        }

        private Response<CommonMessageResult> LabelESetIOExt(Request<SetIOEXTIOMessage, CommonMessageResult> request)
        {
            var myResponse = request.MyResponse;
            IdGrid.Dispatcher.BeginInvoke(new Action<byte, bool>(LabelESetIOExtValue)
                                   , DispatcherPriority.Background, request.RequestContent.IOEXTPort, request.RequestContent.TurneOn);
            LabelDIdCameraLog($"SET IO receive ({request.RequestContent.IOEXTPort} to {request.RequestContent.TurneOn}) request id {request.Id}");
            return myResponse;
        }

        private void LabelESetIOExtValue(byte ioNumber, bool value)
        {
            switch (ioNumber)
            {
                default:
                    break;
                case 0:
                    LabelELabelPrinted.IsChecked = value;
                    break;
                case 1:
                    LabelEWorkpieceOk.IsChecked = value;
                    break;
                case 2:
                    LabelEWorkpieceNok.IsChecked = value;
                    break;
            }
        }

        private Response<LabelDataMessageResult> LabelERead(Request<ReadMessage, LabelDataMessageResult> request)
        {
            string responseContent = LabelEReadResponseComboBox.Dispatcher.Invoke(LabelEGetReadResponseComboText, DispatcherPriority.Normal);
            LabelEIdCameraLog($"Read receive, response with: {responseContent}");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new LabelDataMessageResult(responseContent);
            return myResponse;
        }

        private string LabelEGetReadResponseComboText()
        {
            return LabelEReadResponseComboBox.Text;
        }

        private Response<CommonMessageResult> LabelESetIO(Request<SetIOMessage, CommonMessageResult> request)
        {
            var myResponse = request.MyResponse;
            switch (request.RequestContent.Bulb)
            {
                case Bulbs.Green:
                    LabelDReady.Dispatcher.BeginInvoke(new Action<bool>(LabelESetReady)
                                           , DispatcherPriority.Send, request.RequestContent.BulbTurneOn);
                    break;
                case Bulbs.Red:
                    LabelDLabelPrinted.Dispatcher.BeginInvoke(new Action<bool>(LabelESetIdCameraReady)
                                           , DispatcherPriority.Send, request.RequestContent.BulbTurneOn);
                    break;
                default:
                    break;
            }
            LabelDIdCameraLog($"SET IO receive ({request.RequestContent.Bulb} to {request.RequestContent.BulbTurneOn}) request id {request.Id}");
            return myResponse;
        }

        private void LabelESetReady(bool value)
        {
            LabelEReady.IsChecked = value;
        }

        private void LabelESetIdCameraReady(bool value)
        {
            LabelEIdCameraReady.IsChecked = value;
        }

        private Response<IOChangeMessageResult> LabelEGetIO(Request<GetIOMessage, IOChangeMessageResult> request)
        {
            LabelEIdCameraLog($"GET IO receive");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new IOChangeMessageResult(IdGrid.Dispatcher.Invoke<bool>(() => { return LabelEIdCameraEnable.IsChecked.Value; }, DispatcherPriority.Normal));
            return myResponse;
        }

        private void LabelECheckCameraEnable_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(1, LabelECheckCameraEnable.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelEIdCameraChennelNameTextBox.Text, msg);
            LabelEIdCameraLog($"CheckCamera to {LabelEEnable.IsChecked.Value} IOEXTCAHNGE SEND");
        }

        private void LabelEIdCameraEnable_Click(object sender, RoutedEventArgs e)
        {
            var message = new IOChangeMessageResult(LabelEIdCameraEnable.IsChecked.Value);
            EventHubCore.SendAsync<RedisPubSubChannel, IOChangeMessageResult>(LabelEIdCameraChennelNameTextBox.Text, message);
            LabelEIdCameraLog($"IdCameraEnable to {LabelEIdCameraEnable.IsChecked.Value} IOCAHNGE SEND");
        }

        private void LabelEEnable_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(2, LabelEEnable.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelEIdCameraChennelNameTextBox.Text, msg);
            LabelEIdCameraLog($"Enable to {LabelEEnable.IsChecked.Value} IOEXTCAHNGE SEND");
        }

        private void LabelEStickingDone_Click(object sender, RoutedEventArgs e)
        {
            var msg = new IOEXTIOStateMessageResult(0, LabelEStickingDone.IsChecked.Value);
            EventHubCore.Send<RedisPubSubChannel, IOEXTIOStateMessageResult>(LabelEIdCameraChennelNameTextBox.Text, msg);
            LabelEIdCameraLog($"StickingDone to {LabelEStickingDone.IsChecked.Value} IOEXTCAHNGE SEND");
        }

        #region Log

        private void LabelEIdCameraLog(string line)
        {
            LabelEIdCameraLogListView.Dispatcher.BeginInvoke(new Action<string>(LabelEIdCameraAppendLog)
                                   , DispatcherPriority.Send, line);
        }

        private void LabelEIdCameraAppendLog(string line)
        {
            LabelEIdCameraLogListView.Items.Add(line);
        }

        private void LabelEClearIdCameraLogButton_Click(object sender, RoutedEventArgs e)
        {
            LabelEIdCameraLogListView.Items.Clear();
        }

        #endregion Log

        #endregion LabelE

        #region NOT USED

        private void ManualLabelDataButton_Click(object sender, RoutedEventArgs e)
        {
            ManualLabelDataMessageResult mldmr = new ManualLabelDataMessageResult($"COM; {ManualLabelDataTextBox.Text}");
            EventHubCore.SendAsync<RedisPubSubChannel, ManualLabelDataMessageResult>(LabelDIdCameraChennelNameTextBox.Text, mldmr);
            LabelDIdCameraLog($"MANUAL LABEL DATA SEND: {mldmr.LabelData}");
        }

        #endregion NOT USED

        #endregion IdCamera

        #region CheckCamera

        #region LABELD

        private void LabelDCheckCameraEventHubSubscribe()
        {
            if (LabelDCheckCameraActive.IsChecked.Value)
            {
                try
                {
                    EventHubCore.RegisterHandler<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(LabelDCheckCameraChennelNameTextBox.Text, LabelDCheckCameraRead);
                    LabelDCheckCameraLogListView.Items.Insert(0, $"Check camera emulation started on {LabelDCheckCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelDCheckCameraLogListView.Items.Insert(0, ex.Message);
                }
            }
            else
            {
                try
                {
                    EventHubCore.DropChannel<RedisPubSubChannel>(LabelDCheckCameraChennelNameTextBox.Text);
                    LabelDCheckCameraLogListView.Items.Insert(0, $"Check camera emulation stoped on {LabelDCheckCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelDCheckCameraLogListView.Items.Insert(0, ex.Message);
                }
            }
        }

        private Response<LabelDataMessageResult> LabelDCheckCameraRead(Request<ReadMessage, LabelDataMessageResult> request)
        {
            string responseContent = LabelDReadResponseComboBox.Dispatcher.Invoke(LabelDGetCheckCameraReadResponseComboText, DispatcherPriority.Normal);
            LabelDCheckCameraLog($"Read receive, response with: {responseContent}");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new LabelDataMessageResult(responseContent);
            return myResponse;
        }

        private string LabelDGetCheckCameraReadResponseComboText()
        {
            return LabelDCheckCameraReadResdponseComboBox.Text;
        }

        private void LabelDCheckCameraActive_Click(object sender, RoutedEventArgs e)
        {
            LabelDCheckCameraEventHubSubscribe();
        }

        #region Log

        private void LabelDCheckCameraLog(string line)
        {
            LabelDCheckCameraLogListView.Dispatcher.BeginInvoke(new Action<string>(LabelDCheckCameraAppendLog)
                                   , DispatcherPriority.Background, line);
        }

        private void LabelDCheckCameraAppendLog(string line)
        {
            LabelDCheckCameraLogListView.Items.Add(line);
        }

        private void LabelDClearCheckCameraLogButton_Click(object sender, RoutedEventArgs e)
        {
            LabelDCheckCameraLogListView.Items.Clear();
        }

        #endregion Log

        #endregion LABELD

        #region LABELE

        private void LabelECheckCameraActive_Click(object sender, RoutedEventArgs e)
        {
            LabelECheckCameraEventHubSubscribe();
        }

        private void LabelECheckCameraEventHubSubscribe()
        {
            if (LabelECheckCameraActive.IsChecked.Value)
            {
                try
                {
                    EventHubCore.RegisterHandler<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>(LabelECheckCameraChennelNameTextBox.Text, LabelECheckCameraRead);
                    LabelECheckCameraLogListView.Items.Insert(0, $"Check camera emulation started on {LabelECheckCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelECheckCameraLogListView.Items.Insert(0, ex.Message);
                }
            }
            else
            {
                try
                {
                    EventHubCore.DropChannel<RedisPubSubChannel>(LabelECheckCameraChennelNameTextBox.Text);
                    LabelECheckCameraLogListView.Items.Insert(0, $"Check camera emulation stoped on {LabelECheckCameraChennelNameTextBox.Text} channel");
                }
                catch (Exception ex)
                {
                    LabelECheckCameraLogListView.Items.Insert(0, ex.Message);
                }
            }
        }

        private Response<LabelDataMessageResult> LabelECheckCameraRead(Request<ReadMessage, LabelDataMessageResult> request)
        {
            string responseContent = LabelEReadResponseComboBox.Dispatcher.Invoke(LabelEGetCheckCameraReadResponseComboText, DispatcherPriority.Normal);
            LabelECheckCameraLog($"Read receive, response with: {responseContent}");
            var myResponse = request.MyResponse;
            myResponse.ResponseContent = new LabelDataMessageResult(responseContent);
            return myResponse;
        }

        private string LabelEGetCheckCameraReadResponseComboText()
        {
            return LabelECheckCameraReadResdponseComboBox.Text;
        }

        #region Log

        private void LabelECheckCameraLog(string line)
        {
            LabelECheckCameraLogListView.Dispatcher.BeginInvoke(new Action<string>(LabelECheckCameraAppendLog)
                                   , DispatcherPriority.Background, line);
        }

        private void LabelECheckCameraAppendLog(string line)
        {
            LabelECheckCameraLogListView.Items.Add(line);
        }

        private void LabelEClearCheckCameraLogButton_Click(object sender, RoutedEventArgs e)
        {
            LabelECheckCameraLogListView.Items.Clear();
        }

        #endregion Log

        #endregion LABELE

        #endregion CheckCamera

    }
}
