NDSummary.OnToolTipsLoaded("CSharpClass:DatabaseApplication.CommandProducer",{1:"<div class=\"NDToolTip TClass LCSharp\"><div class=\"NDClassPrototype\" id=\"NDClassPrototype1\"><div class=\"CPEntry TClass Current\"><div class=\"CPName\"><span class=\"Qualifier\">DatabaseApplication.</span>&#8203;CommandProducer</div></div></div><div class=\"TTSummary\">This class will produce commands to the Kafka topic COMMANDS to facilitate IPC</div></div>",61:"<div class=\"NDToolTip TVariable LCSharp\"><div id=\"NDPrototype61\" class=\"NDPrototype NoParameterForm\">ConcurrentQueue&lt;<span class=\"SHKeyword\">string</span>&gt; messageQueue</div><div class=\"TTSummary\">the queue of messages to publish to Kafka.&nbsp; this is shared by the CommandProducer the TwitchCommandConsumer and the TwitchMessageConsumer</div></div>",62:"<div class=\"NDToolTip TVariable LCSharp\"><div id=\"NDPrototype62\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">private bool</span> Active</div><div class=\"TTSummary\">controls the primary loop of CommandProducer</div></div>",63:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype63\" class=\"NDPrototype WideForm CStyle\"><table><tr><td class=\"PBeforeParameters\"><span class=\"SHKeyword\">public</span> CommandProducer(</td><td class=\"PParametersParentCell\"><table class=\"PParameters\"><tr><td class=\"PModifierQualifier first\"><span class=\"SHKeyword\">ref</span>&nbsp;</td><td class=\"PType\">ConcurrentQueue&lt;<span class=\"SHKeyword\">string</span>&gt;&nbsp;</td><td class=\"PName last\">messageQueue</td></tr></table></td><td class=\"PAfterParameters\">)</td></tr></table></div><div class=\"TTSummary\">The constructor for CommandProducer</div></div>",65:"<div class=\"NDToolTip TVariable LCSharp\"><div class=\"TTSummary\">an object containing the configuration settings for the Kafka client</div></div>",260:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype260\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">public void</span> ProducerThread()</div><div class=\"TTSummary\">This is the primary function of CommandConsumer to be run as a thread</div></div>",278:"<div class=\"NDToolTip TVariable LCSharp\"><div class=\"TTSummary\">the number of times the function loops before voleentarily passing control</div></div>",280:"<div class=\"NDToolTip TVariable LCSharp\"><div class=\"TTSummary\">the Kafka producer client</div></div>",282:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype282\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">public void</span> Kill()</div><div class=\"TTSummary\">Ends the loop allowing the thread to close</div></div>"});