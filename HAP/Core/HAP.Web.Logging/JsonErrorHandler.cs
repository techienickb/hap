using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;

namespace HAP.Web.Logging
{
    public class JsonErrorHandler : IErrorHandler
    {
        #region Public Method(s)
        #region IErrorHandler Members
        ///  
        /// Is the error always handled in this class?  
        ///  
        public bool HandleError(Exception error)
        {
            Logging.EventViewer.Log(error.Source, error.Message + "\n\r\n\r" + error.ToString() + "\n\r\n\r" + error.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            return true;
        }

        ///  
        /// Provide the Json fault message  
        ///  
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            fault = this.GetJsonFaultMessage(version, error);

            this.ApplyJsonSettings(ref fault);
            this.ApplyHttpResponseSettings(ref fault, System.Net.HttpStatusCode.BadRequest, "An WCF Error has Occured");
        }
        #endregion
        #endregion

        #region Protected Method(s)
        ///  
        /// Apply Json settings to the message  
        ///  
        protected virtual void ApplyJsonSettings(ref Message fault)
        {
            // Use JSON encoding  
            var jsonFormatting = new WebBodyFormatMessageProperty(WebContentFormat.Json);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, jsonFormatting);
        }

        ///  
        /// Get the HttpResponseMessageProperty  
        ///  
        protected virtual void ApplyHttpResponseSettings(
          ref Message fault, System.Net.HttpStatusCode statusCode,
          string statusDescription)
        {
            var httpResponse = new HttpResponseMessageProperty()
            {
                StatusCode = statusCode,
                StatusDescription = statusDescription
            };
            fault.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
        }

        ///  
        /// Get the json fault message from the provided error  
        ///  
        protected virtual Message GetJsonFaultMessage(MessageVersion version, Exception error)
        {
            BaseFault detail = null;
            var knownTypes = new List<Type>();
            string faultType = error.GetType().Name; //default  

            if ((error is FaultException) &&
                (error.GetType().GetProperty("Detail") != null))
            {
                detail =
                  (error.GetType().GetProperty("Detail").GetGetMethod().Invoke(
                   error, null) as BaseFault);
                knownTypes.Add(detail.GetType());
                faultType = detail.GetType().Name;
            }

            JsonFault jsonFault = new JsonFault
            {
                Message = error.Message,
                Detail = detail,
                FaultType = faultType
            };

            var faultMessage = Message.CreateMessage(version, "", jsonFault,
              new DataContractJsonSerializer(jsonFault.GetType(), knownTypes));

            return faultMessage;
        }
        #endregion
    }

    [DataContract]
    public abstract class BaseFault
    {
        #region Properties
        ///
        /// The fault message
        ///
        [DataMember]
        public string Message
        {
            get;
            set;
        }
        #endregion
    }

    [DataContract]
    public class FaultDetails : BaseFault
    {
    }

    [DataContract]
    public class JsonFault : BaseFault
    {
        #region Properties
        ///
        /// The detail of the fault
        ///
        [DataMember]
        public BaseFault Detail
        {
            get;
            set;
        }

        ///
        /// The type of the fault
        ///
        [DataMember]
        public string FaultType
        {
            get;
            set;
        }
        #endregion
    }

    public class JsonErrorWebHttpBehavior : WebHttpBehavior
    {
        #region Protected Method(s)
        ///  
        /// Add the json error handler to channel error handlers  
        ///  
        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint,
          System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            // clear default error handlers.  
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();

            // add the Json error handler.  
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new JsonErrorHandler());
        }
        #endregion
    }

    public class JsonErrorWebHttpBehaviorElement : BehaviorExtensionElement
    {
        ///  
        /// Get the type of behavior to attach to the endpoint  
        ///  
        public override Type BehaviorType
        {
            get
            {
                return typeof(JsonErrorWebHttpBehavior);
            }
        }

        ///  
        /// Create the custom behavior  
        ///  
        protected override object CreateBehavior()
        {
            return new JsonErrorWebHttpBehavior();
        }
    }  
}