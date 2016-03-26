//using LanguageExt.UnitsOfMeasure;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LanguageExt
//{
//    public class ActorConfig
//    {
//        public readonly ProcessId Pid;
//        public readonly State<StrategyContext, Unit> Strategy = Process.DefaultStrategy;
//        public readonly ProcessFlags Flags = ProcessFlags.Default;
//        public readonly int MailboxSize = ProcessSetting.DefaultMailboxSize;
//        public readonly Map<string, string> Settings = Map.empty<string,string>();

//        internal ActorConfig(IEnumerable<ProcessConfigToken> tokens)
//        {
//            foreach(var token in tokens)
//            {
//                if( token is PidToken)
//                {
//                    Pid = (token as PidToken).Pid;
//                }
//                else if (token is FlagsToken)
//                {
//                    Flags = (token as FlagsToken).Flags;
//                }
//                else if (token is StrategyToken)
//                {
//                    Strategy = (token as StrategyToken).Strategy;
//                }
//                else if (token is SettingsToken)
//                {
//                    Settings = (token as SettingsToken).Settings;
//                }
//                else if (token is MailboxSizeToken)
//                {
//                    MailboxSize = (token as MailboxSizeToken).Size;
//                }
//            }
//        }
//    }
//}
