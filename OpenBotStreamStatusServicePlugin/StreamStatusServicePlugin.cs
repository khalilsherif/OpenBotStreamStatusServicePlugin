using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBot.Plugins;
using OpenBot.Plugins.Interfaces;
using OpenBotStreamStatusServicePlugin.Services;

namespace OpenBotStreamStatusServicePlugin
{
    public class StreamStatusServicePlugin : AbstractPlugin
    {
        public override IChatHandlerFactory[] ChatHandlerFactories
        {
            get
            {
                return null;
            }
        }

        public override string Description
        {
            get
            {
                return "Exposes a service, when paired with the CLR Host plugin, can accurately provide information on stream start, end, and duration times.";
            }
        }

        public override string Name
        {
            get
            {
                return "Stream Status";
            }
        }

        public override IServiceFactory[] ServiceFactories
        {
            get
            {
                return new IServiceFactory[]
                {
                    new DefaultServiceFactory<StreamStatusService>()
                };
            }
        }

        public override bool LoadPlugin()
        {
            return true;
        }

        public override void UnloadPlugin()
        {
            
        }
    }
}
