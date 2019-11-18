NDSummary.OnToolTipsLoaded("CSharpClass:StreamClientProducer.KafkaProducer",{222:"<div class=\"NDToolTip TClass LCSharp\"><div class=\"NDClassPrototype\" id=\"NDClassPrototype222\"><div class=\"CPEntry TClass Current\"><div class=\"CPName\"><span class=\"Qualifier\">StreamClientProducer.</span>&#8203;KafkaProducer</div></div></div><div class=\"TTSummary\">This class is responcible for dequeue messages from messageQueue and then publishing them to the kafka server.</div></div>",243:"<div class=\"NDToolTip TVariable LCSharp\"><div id=\"NDPrototype243\" class=\"NDPrototype NoParameterForm\">ConcurrentQueue&lt;String&gt; messageQueue</div><div class=\"TTSummary\">The shared queue that contains all the incoming messages from twitch</div></div>",244:"<div class=\"NDToolTip TVariable LCSharp\"><div id=\"NDPrototype244\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">private bool</span> Active</div><div class=\"TTSummary\">manages the main loop for this object</div></div>",255:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype255\" class=\"NDPrototype WideForm CStyle\"><table><tr><td class=\"PBeforeParameters\"><span class=\"SHKeyword\">public</span> KafkaProducer(</td><td class=\"PParametersParentCell\"><table class=\"PParameters\"><tr><td class=\"PModifierQualifier first\"><span class=\"SHKeyword\">ref</span>&nbsp;</td><td class=\"PType\">ConcurrentQueue&lt;String&gt;&nbsp;</td><td class=\"PName last\">messageQueue</td></tr></table></td><td class=\"PAfterParameters\">)</td></tr></table></div><div class=\"TTSummary\">The coinstructor for KafkaProducer</div></div>",256:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype256\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">public void</span> ProducerThread()</div><div class=\"TTSummary\">This is the primary fuction and run as a thread by ThreadController</div></div>",257:"<div class=\"NDToolTip TFunction LCSharp\"><div id=\"NDPrototype257\" class=\"NDPrototype NoParameterForm\"><span class=\"SHKeyword\">public async</span> Task KillAsync()</div><div class=\"TTSummary\">This function closes the thread after it has finished transmitting all messages in the queue to Kafka</div></div>"});