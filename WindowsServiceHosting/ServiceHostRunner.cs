namespace WindowsServiceHosting
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using WindowsServiceHosting.Loggers;

    public class ServiceHostRunner
    {
        private readonly ServiceHost serviceHost;
        private readonly ILogger messagesContainer;
        private readonly ManualResetEvent serviceHostResetEvent = new ManualResetEvent(false);

        public ServiceHostRunner(ServiceHost serviceHost)
        {
            if (serviceHost == null)
            {
                throw new ArgumentNullException("serviceHost");
            }

            this.serviceHost = serviceHost;
        }

        public ServiceHostRunner(ServiceHost serviceHost, ILogger messagesContainer)
            : this(serviceHost)
        {
            this.messagesContainer = messagesContainer;
        }

        public ServiceHostRunnerState State
        {
            get
            {
                if (this.serviceHost.State == CommunicationState.Created
                    || this.serviceHost.State == CommunicationState.Opened
                    || this.serviceHost.State == CommunicationState.Closing)
                {
                    return ServiceHostRunnerState.Running;
                }

                return ServiceHostRunnerState.Stopped;
            }
        }

        public void Start()
        {
            this.serviceHost.BeginOpen(this.OnOpenComplete, this.serviceHost);
        }

        public void Stop()
        {
            this.serviceHostResetEvent.Set();
        }

        private void OnOpenComplete(IAsyncResult asyncResult)
        {
            var host = (ServiceHost)asyncResult.AsyncState;

            try
            {
                this.serviceHostResetEvent.Reset();

                host.EndOpen(asyncResult);

                this.WriteMessage(string.Format("Host for {0} is running", host.Description.ServiceType));
                this.WriteMessage(string.Empty);

                this.serviceHostResetEvent.WaitOne();
            }
            catch (CommunicationException exception)
            {
                this.WriteError(string.Format("Service host opening for {0} failed.", host.Description.ServiceType));
                this.WriteMessage(string.Empty);
                this.WriteError(exception.Message);
                this.WriteError(exception.StackTrace);
            }
            finally
            {
                if (host.State == CommunicationState.Opened)
                {
                    host.BeginClose(this.OnCloseComplete, host);
                }
            }
        }

        private void OnCloseComplete(IAsyncResult asyncResult)
        {
            var host = (ServiceHost)asyncResult.AsyncState;

            try
            {
                host.EndClose(asyncResult);
            }
            catch (CommunicationException)
            {
                this.WriteError(string.Format("An error occured on closing host for {0} service.", host.Description.ServiceType));
                host.Abort();
            }

            this.WriteMessage(string.Format("Host for {0} has been stopped.", host.Description.ServiceType));
        }

        private void WriteMessage(string message)
        {
            if (this.messagesContainer != null)
            {
                this.messagesContainer.AddMessage(message);
            }
        }

        private void WriteError(string message)
        {
            if (this.messagesContainer != null)
            {
                this.messagesContainer.AddError(message);
            }
        }
    }
}