using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal static class TextGeneratorUtilities
{
	public static readonly Vector2 largePositiveVector2 = new Vector2(2.1474836E+09f, 2.1474836E+09f);

	public static readonly Vector2 largeNegativeVector2 = new Vector2(-214748370f, -214748370f);

	public const float largePositiveFloat = 32767f;

	public const float largeNegativeFloat = -32767f;

	private const int k_DoubleQuotes = 34;

	private const int k_GreaterThan = 62;

	private const int k_ZeroWidthSpace = 8203;

	private const string k_LookupStringU = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-";

	private static readonly HashSet<uint> k_EmojiLookup = new HashSet<uint>(new uint[1431]
	{
		35u, 42u, 48u, 49u, 50u, 51u, 52u, 53u, 54u, 55u,
		56u, 57u, 169u, 174u, 8252u, 8265u, 8482u, 8505u, 8596u, 8597u,
		8598u, 8599u, 8600u, 8601u, 8617u, 8618u, 8986u, 8987u, 9000u, 9167u,
		9193u, 9194u, 9195u, 9196u, 9197u, 9198u, 9199u, 9200u, 9201u, 9202u,
		9203u, 9208u, 9209u, 9210u, 9410u, 9642u, 9643u, 9654u, 9664u, 9723u,
		9724u, 9725u, 9726u, 9728u, 9729u, 9730u, 9731u, 9732u, 9742u, 9745u,
		9748u, 9749u, 9752u, 9757u, 9760u, 9762u, 9763u, 9766u, 9770u, 9774u,
		9775u, 9784u, 9785u, 9786u, 9792u, 9794u, 9800u, 9801u, 9802u, 9803u,
		9804u, 9805u, 9806u, 9807u, 9808u, 9809u, 9810u, 9811u, 9823u, 9824u,
		9827u, 9829u, 9830u, 9832u, 9851u, 9854u, 9855u, 9874u, 9875u, 9876u,
		9877u, 9878u, 9879u, 9881u, 9883u, 9884u, 9888u, 9889u, 9895u, 9898u,
		9899u, 9904u, 9905u, 9917u, 9918u, 9924u, 9925u, 9928u, 9934u, 9935u,
		9937u, 9939u, 9940u, 9961u, 9962u, 9968u, 9969u, 9970u, 9971u, 9972u,
		9973u, 9975u, 9976u, 9977u, 9978u, 9981u, 9986u, 9989u, 9992u, 9993u,
		9994u, 9995u, 9996u, 9997u, 9999u, 10002u, 10004u, 10006u, 10013u, 10017u,
		10024u, 10035u, 10036u, 10052u, 10055u, 10060u, 10062u, 10067u, 10068u, 10069u,
		10071u, 10083u, 10084u, 10133u, 10134u, 10135u, 10145u, 10160u, 10175u, 10548u,
		10549u, 11013u, 11014u, 11015u, 11035u, 11036u, 11088u, 11093u, 12336u, 12349u,
		12951u, 12953u, 126980u, 127183u, 127344u, 127345u, 127358u, 127359u, 127374u, 127377u,
		127378u, 127379u, 127380u, 127381u, 127382u, 127383u, 127384u, 127385u, 127386u, 127462u,
		127463u, 127464u, 127465u, 127466u, 127467u, 127468u, 127469u, 127470u, 127471u, 127472u,
		127473u, 127474u, 127475u, 127476u, 127477u, 127478u, 127479u, 127480u, 127481u, 127482u,
		127483u, 127484u, 127485u, 127486u, 127487u, 127489u, 127490u, 127514u, 127535u, 127538u,
		127539u, 127540u, 127541u, 127542u, 127543u, 127544u, 127545u, 127546u, 127568u, 127569u,
		127744u, 127745u, 127746u, 127747u, 127748u, 127749u, 127750u, 127751u, 127752u, 127753u,
		127754u, 127755u, 127756u, 127757u, 127758u, 127759u, 127760u, 127761u, 127762u, 127763u,
		127764u, 127765u, 127766u, 127767u, 127768u, 127769u, 127770u, 127771u, 127772u, 127773u,
		127774u, 127775u, 127776u, 127777u, 127780u, 127781u, 127782u, 127783u, 127784u, 127785u,
		127786u, 127787u, 127788u, 127789u, 127790u, 127791u, 127792u, 127793u, 127794u, 127795u,
		127796u, 127797u, 127798u, 127799u, 127800u, 127801u, 127802u, 127803u, 127804u, 127805u,
		127806u, 127807u, 127808u, 127809u, 127810u, 127811u, 127812u, 127813u, 127814u, 127815u,
		127816u, 127817u, 127818u, 127819u, 127820u, 127821u, 127822u, 127823u, 127824u, 127825u,
		127826u, 127827u, 127828u, 127829u, 127830u, 127831u, 127832u, 127833u, 127834u, 127835u,
		127836u, 127837u, 127838u, 127839u, 127840u, 127841u, 127842u, 127843u, 127844u, 127845u,
		127846u, 127847u, 127848u, 127849u, 127850u, 127851u, 127852u, 127853u, 127854u, 127855u,
		127856u, 127857u, 127858u, 127859u, 127860u, 127861u, 127862u, 127863u, 127864u, 127865u,
		127866u, 127867u, 127868u, 127869u, 127870u, 127871u, 127872u, 127873u, 127874u, 127875u,
		127876u, 127877u, 127878u, 127879u, 127880u, 127881u, 127882u, 127883u, 127884u, 127885u,
		127886u, 127887u, 127888u, 127889u, 127890u, 127891u, 127894u, 127895u, 127897u, 127898u,
		127899u, 127902u, 127903u, 127904u, 127905u, 127906u, 127907u, 127908u, 127909u, 127910u,
		127911u, 127912u, 127913u, 127914u, 127915u, 127916u, 127917u, 127918u, 127919u, 127920u,
		127921u, 127922u, 127923u, 127924u, 127925u, 127926u, 127927u, 127928u, 127929u, 127930u,
		127931u, 127932u, 127933u, 127934u, 127935u, 127936u, 127937u, 127938u, 127939u, 127940u,
		127941u, 127942u, 127943u, 127944u, 127945u, 127946u, 127947u, 127948u, 127949u, 127950u,
		127951u, 127952u, 127953u, 127954u, 127955u, 127956u, 127957u, 127958u, 127959u, 127960u,
		127961u, 127962u, 127963u, 127964u, 127965u, 127966u, 127967u, 127968u, 127969u, 127970u,
		127971u, 127972u, 127973u, 127974u, 127975u, 127976u, 127977u, 127978u, 127979u, 127980u,
		127981u, 127982u, 127983u, 127984u, 127987u, 127988u, 127989u, 127991u, 127992u, 127993u,
		127994u, 127995u, 127996u, 127997u, 127998u, 127999u, 128000u, 128001u, 128002u, 128003u,
		128004u, 128005u, 128006u, 128007u, 128008u, 128009u, 128010u, 128011u, 128012u, 128013u,
		128014u, 128015u, 128016u, 128017u, 128018u, 128019u, 128020u, 128021u, 128022u, 128023u,
		128024u, 128025u, 128026u, 128027u, 128028u, 128029u, 128030u, 128031u, 128032u, 128033u,
		128034u, 128035u, 128036u, 128037u, 128038u, 128039u, 128040u, 128041u, 128042u, 128043u,
		128044u, 128045u, 128046u, 128047u, 128048u, 128049u, 128050u, 128051u, 128052u, 128053u,
		128054u, 128055u, 128056u, 128057u, 128058u, 128059u, 128060u, 128061u, 128062u, 128063u,
		128064u, 128065u, 128066u, 128067u, 128068u, 128069u, 128070u, 128071u, 128072u, 128073u,
		128074u, 128075u, 128076u, 128077u, 128078u, 128079u, 128080u, 128081u, 128082u, 128083u,
		128084u, 128085u, 128086u, 128087u, 128088u, 128089u, 128090u, 128091u, 128092u, 128093u,
		128094u, 128095u, 128096u, 128097u, 128098u, 128099u, 128100u, 128101u, 128102u, 128103u,
		128104u, 128105u, 128106u, 128107u, 128108u, 128109u, 128110u, 128111u, 128112u, 128113u,
		128114u, 128115u, 128116u, 128117u, 128118u, 128119u, 128120u, 128121u, 128122u, 128123u,
		128124u, 128125u, 128126u, 128127u, 128128u, 128129u, 128130u, 128131u, 128132u, 128133u,
		128134u, 128135u, 128136u, 128137u, 128138u, 128139u, 128140u, 128141u, 128142u, 128143u,
		128144u, 128145u, 128146u, 128147u, 128148u, 128149u, 128150u, 128151u, 128152u, 128153u,
		128154u, 128155u, 128156u, 128157u, 128158u, 128159u, 128160u, 128161u, 128162u, 128163u,
		128164u, 128165u, 128166u, 128167u, 128168u, 128169u, 128170u, 128171u, 128172u, 128173u,
		128174u, 128175u, 128176u, 128177u, 128178u, 128179u, 128180u, 128181u, 128182u, 128183u,
		128184u, 128185u, 128186u, 128187u, 128188u, 128189u, 128190u, 128191u, 128192u, 128193u,
		128194u, 128195u, 128196u, 128197u, 128198u, 128199u, 128200u, 128201u, 128202u, 128203u,
		128204u, 128205u, 128206u, 128207u, 128208u, 128209u, 128210u, 128211u, 128212u, 128213u,
		128214u, 128215u, 128216u, 128217u, 128218u, 128219u, 128220u, 128221u, 128222u, 128223u,
		128224u, 128225u, 128226u, 128227u, 128228u, 128229u, 128230u, 128231u, 128232u, 128233u,
		128234u, 128235u, 128236u, 128237u, 128238u, 128239u, 128240u, 128241u, 128242u, 128243u,
		128244u, 128245u, 128246u, 128247u, 128248u, 128249u, 128250u, 128251u, 128252u, 128253u,
		128255u, 128256u, 128257u, 128258u, 128259u, 128260u, 128261u, 128262u, 128263u, 128264u,
		128265u, 128266u, 128267u, 128268u, 128269u, 128270u, 128271u, 128272u, 128273u, 128274u,
		128275u, 128276u, 128277u, 128278u, 128279u, 128280u, 128281u, 128282u, 128283u, 128284u,
		128285u, 128286u, 128287u, 128288u, 128289u, 128290u, 128291u, 128292u, 128293u, 128294u,
		128295u, 128296u, 128297u, 128298u, 128299u, 128300u, 128301u, 128302u, 128303u, 128304u,
		128305u, 128306u, 128307u, 128308u, 128309u, 128310u, 128311u, 128312u, 128313u, 128314u,
		128315u, 128316u, 128317u, 128329u, 128330u, 128331u, 128332u, 128333u, 128334u, 128336u,
		128337u, 128338u, 128339u, 128340u, 128341u, 128342u, 128343u, 128344u, 128345u, 128346u,
		128347u, 128348u, 128349u, 128350u, 128351u, 128352u, 128353u, 128354u, 128355u, 128356u,
		128357u, 128358u, 128359u, 128367u, 128368u, 128371u, 128372u, 128373u, 128374u, 128375u,
		128376u, 128377u, 128378u, 128391u, 128394u, 128395u, 128396u, 128397u, 128400u, 128405u,
		128406u, 128420u, 128421u, 128424u, 128433u, 128434u, 128444u, 128450u, 128451u, 128452u,
		128465u, 128466u, 128467u, 128476u, 128477u, 128478u, 128481u, 128483u, 128488u, 128495u,
		128499u, 128506u, 128507u, 128508u, 128509u, 128510u, 128511u, 128512u, 128513u, 128514u,
		128515u, 128516u, 128517u, 128518u, 128519u, 128520u, 128521u, 128522u, 128523u, 128524u,
		128525u, 128526u, 128527u, 128528u, 128529u, 128530u, 128531u, 128532u, 128533u, 128534u,
		128535u, 128536u, 128537u, 128538u, 128539u, 128540u, 128541u, 128542u, 128543u, 128544u,
		128545u, 128546u, 128547u, 128548u, 128549u, 128550u, 128551u, 128552u, 128553u, 128554u,
		128555u, 128556u, 128557u, 128558u, 128559u, 128560u, 128561u, 128562u, 128563u, 128564u,
		128565u, 128566u, 128567u, 128568u, 128569u, 128570u, 128571u, 128572u, 128573u, 128574u,
		128575u, 128576u, 128577u, 128578u, 128579u, 128580u, 128581u, 128582u, 128583u, 128584u,
		128585u, 128586u, 128587u, 128588u, 128589u, 128590u, 128591u, 128640u, 128641u, 128642u,
		128643u, 128644u, 128645u, 128646u, 128647u, 128648u, 128649u, 128650u, 128651u, 128652u,
		128653u, 128654u, 128655u, 128656u, 128657u, 128658u, 128659u, 128660u, 128661u, 128662u,
		128663u, 128664u, 128665u, 128666u, 128667u, 128668u, 128669u, 128670u, 128671u, 128672u,
		128673u, 128674u, 128675u, 128676u, 128677u, 128678u, 128679u, 128680u, 128681u, 128682u,
		128683u, 128684u, 128685u, 128686u, 128687u, 128688u, 128689u, 128690u, 128691u, 128692u,
		128693u, 128694u, 128695u, 128696u, 128697u, 128698u, 128699u, 128700u, 128701u, 128702u,
		128703u, 128704u, 128705u, 128706u, 128707u, 128708u, 128709u, 128715u, 128716u, 128717u,
		128718u, 128719u, 128720u, 128721u, 128722u, 128725u, 128726u, 128727u, 128732u, 128733u,
		128734u, 128735u, 128736u, 128737u, 128738u, 128739u, 128740u, 128741u, 128745u, 128747u,
		128748u, 128752u, 128755u, 128756u, 128757u, 128758u, 128759u, 128760u, 128761u, 128762u,
		128763u, 128764u, 128992u, 128993u, 128994u, 128995u, 128996u, 128997u, 128998u, 128999u,
		129000u, 129001u, 129002u, 129003u, 129008u, 129292u, 129293u, 129294u, 129295u, 129296u,
		129297u, 129298u, 129299u, 129300u, 129301u, 129302u, 129303u, 129304u, 129305u, 129306u,
		129307u, 129308u, 129309u, 129310u, 129311u, 129312u, 129313u, 129314u, 129315u, 129316u,
		129317u, 129318u, 129319u, 129320u, 129321u, 129322u, 129323u, 129324u, 129325u, 129326u,
		129327u, 129328u, 129329u, 129330u, 129331u, 129332u, 129333u, 129334u, 129335u, 129336u,
		129337u, 129338u, 129340u, 129341u, 129342u, 129343u, 129344u, 129345u, 129346u, 129347u,
		129348u, 129349u, 129351u, 129352u, 129353u, 129354u, 129355u, 129356u, 129357u, 129358u,
		129359u, 129360u, 129361u, 129362u, 129363u, 129364u, 129365u, 129366u, 129367u, 129368u,
		129369u, 129370u, 129371u, 129372u, 129373u, 129374u, 129375u, 129376u, 129377u, 129378u,
		129379u, 129380u, 129381u, 129382u, 129383u, 129384u, 129385u, 129386u, 129387u, 129388u,
		129389u, 129390u, 129391u, 129392u, 129393u, 129394u, 129395u, 129396u, 129397u, 129398u,
		129399u, 129400u, 129401u, 129402u, 129403u, 129404u, 129405u, 129406u, 129407u, 129408u,
		129409u, 129410u, 129411u, 129412u, 129413u, 129414u, 129415u, 129416u, 129417u, 129418u,
		129419u, 129420u, 129421u, 129422u, 129423u, 129424u, 129425u, 129426u, 129427u, 129428u,
		129429u, 129430u, 129431u, 129432u, 129433u, 129434u, 129435u, 129436u, 129437u, 129438u,
		129439u, 129440u, 129441u, 129442u, 129443u, 129444u, 129445u, 129446u, 129447u, 129448u,
		129449u, 129450u, 129451u, 129452u, 129453u, 129454u, 129455u, 129456u, 129457u, 129458u,
		129459u, 129460u, 129461u, 129462u, 129463u, 129464u, 129465u, 129466u, 129467u, 129468u,
		129469u, 129470u, 129471u, 129472u, 129473u, 129474u, 129475u, 129476u, 129477u, 129478u,
		129479u, 129480u, 129481u, 129482u, 129483u, 129484u, 129485u, 129486u, 129487u, 129488u,
		129489u, 129490u, 129491u, 129492u, 129493u, 129494u, 129495u, 129496u, 129497u, 129498u,
		129499u, 129500u, 129501u, 129502u, 129503u, 129504u, 129505u, 129506u, 129507u, 129508u,
		129509u, 129510u, 129511u, 129512u, 129513u, 129514u, 129515u, 129516u, 129517u, 129518u,
		129519u, 129520u, 129521u, 129522u, 129523u, 129524u, 129525u, 129526u, 129527u, 129528u,
		129529u, 129530u, 129531u, 129532u, 129533u, 129534u, 129535u, 129648u, 129649u, 129650u,
		129651u, 129652u, 129653u, 129654u, 129655u, 129656u, 129657u, 129658u, 129659u, 129660u,
		129664u, 129665u, 129666u, 129667u, 129668u, 129669u, 129670u, 129671u, 129672u, 129673u,
		129679u, 129680u, 129681u, 129682u, 129683u, 129684u, 129685u, 129686u, 129687u, 129688u,
		129689u, 129690u, 129691u, 129692u, 129693u, 129694u, 129695u, 129696u, 129697u, 129698u,
		129699u, 129700u, 129701u, 129702u, 129703u, 129704u, 129705u, 129706u, 129707u, 129708u,
		129709u, 129710u, 129711u, 129712u, 129713u, 129714u, 129715u, 129716u, 129717u, 129718u,
		129719u, 129720u, 129721u, 129722u, 129723u, 129724u, 129725u, 129726u, 129727u, 129728u,
		129729u, 129730u, 129731u, 129732u, 129733u, 129734u, 129742u, 129743u, 129744u, 129745u,
		129746u, 129747u, 129748u, 129749u, 129750u, 129751u, 129752u, 129753u, 129754u, 129755u,
		129756u, 129759u, 129760u, 129761u, 129762u, 129763u, 129764u, 129765u, 129766u, 129767u,
		129768u, 129769u, 129776u, 129777u, 129778u, 129779u, 129780u, 129781u, 129782u, 129783u,
		129784u
	});

	private static readonly HashSet<uint> k_EmojiPresentationFormLookup = new HashSet<uint>(new uint[1212]
	{
		8986u, 8987u, 9193u, 9194u, 9195u, 9196u, 9200u, 9203u, 9725u, 9726u,
		9748u, 9749u, 9800u, 9801u, 9802u, 9803u, 9804u, 9805u, 9806u, 9807u,
		9808u, 9809u, 9810u, 9811u, 9855u, 9875u, 9889u, 9898u, 9899u, 9917u,
		9918u, 9924u, 9925u, 9934u, 9940u, 9962u, 9970u, 9971u, 9973u, 9978u,
		9981u, 9989u, 9994u, 9995u, 10024u, 10060u, 10062u, 10067u, 10068u, 10069u,
		10071u, 10133u, 10134u, 10135u, 10160u, 10175u, 11035u, 11036u, 11088u, 11093u,
		126980u, 127183u, 127374u, 127377u, 127378u, 127379u, 127380u, 127381u, 127382u, 127383u,
		127384u, 127385u, 127386u, 127462u, 127463u, 127464u, 127465u, 127466u, 127467u, 127468u,
		127469u, 127470u, 127471u, 127472u, 127473u, 127474u, 127475u, 127476u, 127477u, 127478u,
		127479u, 127480u, 127481u, 127482u, 127483u, 127484u, 127485u, 127486u, 127487u, 127489u,
		127514u, 127535u, 127538u, 127539u, 127540u, 127541u, 127542u, 127544u, 127545u, 127546u,
		127568u, 127569u, 127744u, 127745u, 127746u, 127747u, 127748u, 127749u, 127750u, 127751u,
		127752u, 127753u, 127754u, 127755u, 127756u, 127757u, 127758u, 127759u, 127760u, 127761u,
		127762u, 127763u, 127764u, 127765u, 127766u, 127767u, 127768u, 127769u, 127770u, 127771u,
		127772u, 127773u, 127774u, 127775u, 127776u, 127789u, 127790u, 127791u, 127792u, 127793u,
		127794u, 127795u, 127796u, 127797u, 127799u, 127800u, 127801u, 127802u, 127803u, 127804u,
		127805u, 127806u, 127807u, 127808u, 127809u, 127810u, 127811u, 127812u, 127813u, 127814u,
		127815u, 127816u, 127817u, 127818u, 127819u, 127820u, 127821u, 127822u, 127823u, 127824u,
		127825u, 127826u, 127827u, 127828u, 127829u, 127830u, 127831u, 127832u, 127833u, 127834u,
		127835u, 127836u, 127837u, 127838u, 127839u, 127840u, 127841u, 127842u, 127843u, 127844u,
		127845u, 127846u, 127847u, 127848u, 127849u, 127850u, 127851u, 127852u, 127853u, 127854u,
		127855u, 127856u, 127857u, 127858u, 127859u, 127860u, 127861u, 127862u, 127863u, 127864u,
		127865u, 127866u, 127867u, 127868u, 127870u, 127871u, 127872u, 127873u, 127874u, 127875u,
		127876u, 127877u, 127878u, 127879u, 127880u, 127881u, 127882u, 127883u, 127884u, 127885u,
		127886u, 127887u, 127888u, 127889u, 127890u, 127891u, 127904u, 127905u, 127906u, 127907u,
		127908u, 127909u, 127910u, 127911u, 127912u, 127913u, 127914u, 127915u, 127916u, 127917u,
		127918u, 127919u, 127920u, 127921u, 127922u, 127923u, 127924u, 127925u, 127926u, 127927u,
		127928u, 127929u, 127930u, 127931u, 127932u, 127933u, 127934u, 127935u, 127936u, 127937u,
		127938u, 127939u, 127940u, 127941u, 127942u, 127943u, 127944u, 127945u, 127946u, 127951u,
		127952u, 127953u, 127954u, 127955u, 127968u, 127969u, 127970u, 127971u, 127972u, 127973u,
		127974u, 127975u, 127976u, 127977u, 127978u, 127979u, 127980u, 127981u, 127982u, 127983u,
		127984u, 127988u, 127992u, 127993u, 127994u, 127995u, 127996u, 127997u, 127998u, 127999u,
		128000u, 128001u, 128002u, 128003u, 128004u, 128005u, 128006u, 128007u, 128008u, 128009u,
		128010u, 128011u, 128012u, 128013u, 128014u, 128015u, 128016u, 128017u, 128018u, 128019u,
		128020u, 128021u, 128022u, 128023u, 128024u, 128025u, 128026u, 128027u, 128028u, 128029u,
		128030u, 128031u, 128032u, 128033u, 128034u, 128035u, 128036u, 128037u, 128038u, 128039u,
		128040u, 128041u, 128042u, 128043u, 128044u, 128045u, 128046u, 128047u, 128048u, 128049u,
		128050u, 128051u, 128052u, 128053u, 128054u, 128055u, 128056u, 128057u, 128058u, 128059u,
		128060u, 128061u, 128062u, 128064u, 128066u, 128067u, 128068u, 128069u, 128070u, 128071u,
		128072u, 128073u, 128074u, 128075u, 128076u, 128077u, 128078u, 128079u, 128080u, 128081u,
		128082u, 128083u, 128084u, 128085u, 128086u, 128087u, 128088u, 128089u, 128090u, 128091u,
		128092u, 128093u, 128094u, 128095u, 128096u, 128097u, 128098u, 128099u, 128100u, 128101u,
		128102u, 128103u, 128104u, 128105u, 128106u, 128107u, 128108u, 128109u, 128110u, 128111u,
		128112u, 128113u, 128114u, 128115u, 128116u, 128117u, 128118u, 128119u, 128120u, 128121u,
		128122u, 128123u, 128124u, 128125u, 128126u, 128127u, 128128u, 128129u, 128130u, 128131u,
		128132u, 128133u, 128134u, 128135u, 128136u, 128137u, 128138u, 128139u, 128140u, 128141u,
		128142u, 128143u, 128144u, 128145u, 128146u, 128147u, 128148u, 128149u, 128150u, 128151u,
		128152u, 128153u, 128154u, 128155u, 128156u, 128157u, 128158u, 128159u, 128160u, 128161u,
		128162u, 128163u, 128164u, 128165u, 128166u, 128167u, 128168u, 128169u, 128170u, 128171u,
		128172u, 128173u, 128174u, 128175u, 128176u, 128177u, 128178u, 128179u, 128180u, 128181u,
		128182u, 128183u, 128184u, 128185u, 128186u, 128187u, 128188u, 128189u, 128190u, 128191u,
		128192u, 128193u, 128194u, 128195u, 128196u, 128197u, 128198u, 128199u, 128200u, 128201u,
		128202u, 128203u, 128204u, 128205u, 128206u, 128207u, 128208u, 128209u, 128210u, 128211u,
		128212u, 128213u, 128214u, 128215u, 128216u, 128217u, 128218u, 128219u, 128220u, 128221u,
		128222u, 128223u, 128224u, 128225u, 128226u, 128227u, 128228u, 128229u, 128230u, 128231u,
		128232u, 128233u, 128234u, 128235u, 128236u, 128237u, 128238u, 128239u, 128240u, 128241u,
		128242u, 128243u, 128244u, 128245u, 128246u, 128247u, 128248u, 128249u, 128250u, 128251u,
		128252u, 128255u, 128256u, 128257u, 128258u, 128259u, 128260u, 128261u, 128262u, 128263u,
		128264u, 128265u, 128266u, 128267u, 128268u, 128269u, 128270u, 128271u, 128272u, 128273u,
		128274u, 128275u, 128276u, 128277u, 128278u, 128279u, 128280u, 128281u, 128282u, 128283u,
		128284u, 128285u, 128286u, 128287u, 128288u, 128289u, 128290u, 128291u, 128292u, 128293u,
		128294u, 128295u, 128296u, 128297u, 128298u, 128299u, 128300u, 128301u, 128302u, 128303u,
		128304u, 128305u, 128306u, 128307u, 128308u, 128309u, 128310u, 128311u, 128312u, 128313u,
		128314u, 128315u, 128316u, 128317u, 128331u, 128332u, 128333u, 128334u, 128336u, 128337u,
		128338u, 128339u, 128340u, 128341u, 128342u, 128343u, 128344u, 128345u, 128346u, 128347u,
		128348u, 128349u, 128350u, 128351u, 128352u, 128353u, 128354u, 128355u, 128356u, 128357u,
		128358u, 128359u, 128378u, 128405u, 128406u, 128420u, 128507u, 128508u, 128509u, 128510u,
		128511u, 128512u, 128513u, 128514u, 128515u, 128516u, 128517u, 128518u, 128519u, 128520u,
		128521u, 128522u, 128523u, 128524u, 128525u, 128526u, 128527u, 128528u, 128529u, 128530u,
		128531u, 128532u, 128533u, 128534u, 128535u, 128536u, 128537u, 128538u, 128539u, 128540u,
		128541u, 128542u, 128543u, 128544u, 128545u, 128546u, 128547u, 128548u, 128549u, 128550u,
		128551u, 128552u, 128553u, 128554u, 128555u, 128556u, 128557u, 128558u, 128559u, 128560u,
		128561u, 128562u, 128563u, 128564u, 128565u, 128566u, 128567u, 128568u, 128569u, 128570u,
		128571u, 128572u, 128573u, 128574u, 128575u, 128576u, 128577u, 128578u, 128579u, 128580u,
		128581u, 128582u, 128583u, 128584u, 128585u, 128586u, 128587u, 128588u, 128589u, 128590u,
		128591u, 128640u, 128641u, 128642u, 128643u, 128644u, 128645u, 128646u, 128647u, 128648u,
		128649u, 128650u, 128651u, 128652u, 128653u, 128654u, 128655u, 128656u, 128657u, 128658u,
		128659u, 128660u, 128661u, 128662u, 128663u, 128664u, 128665u, 128666u, 128667u, 128668u,
		128669u, 128670u, 128671u, 128672u, 128673u, 128674u, 128675u, 128676u, 128677u, 128678u,
		128679u, 128680u, 128681u, 128682u, 128683u, 128684u, 128685u, 128686u, 128687u, 128688u,
		128689u, 128690u, 128691u, 128692u, 128693u, 128694u, 128695u, 128696u, 128697u, 128698u,
		128699u, 128700u, 128701u, 128702u, 128703u, 128704u, 128705u, 128706u, 128707u, 128708u,
		128709u, 128716u, 128720u, 128721u, 128722u, 128725u, 128726u, 128727u, 128732u, 128733u,
		128734u, 128735u, 128747u, 128748u, 128756u, 128757u, 128758u, 128759u, 128760u, 128761u,
		128762u, 128763u, 128764u, 128992u, 128993u, 128994u, 128995u, 128996u, 128997u, 128998u,
		128999u, 129000u, 129001u, 129002u, 129003u, 129008u, 129292u, 129293u, 129294u, 129295u,
		129296u, 129297u, 129298u, 129299u, 129300u, 129301u, 129302u, 129303u, 129304u, 129305u,
		129306u, 129307u, 129308u, 129309u, 129310u, 129311u, 129312u, 129313u, 129314u, 129315u,
		129316u, 129317u, 129318u, 129319u, 129320u, 129321u, 129322u, 129323u, 129324u, 129325u,
		129326u, 129327u, 129328u, 129329u, 129330u, 129331u, 129332u, 129333u, 129334u, 129335u,
		129336u, 129337u, 129338u, 129340u, 129341u, 129342u, 129343u, 129344u, 129345u, 129346u,
		129347u, 129348u, 129349u, 129351u, 129352u, 129353u, 129354u, 129355u, 129356u, 129357u,
		129358u, 129359u, 129360u, 129361u, 129362u, 129363u, 129364u, 129365u, 129366u, 129367u,
		129368u, 129369u, 129370u, 129371u, 129372u, 129373u, 129374u, 129375u, 129376u, 129377u,
		129378u, 129379u, 129380u, 129381u, 129382u, 129383u, 129384u, 129385u, 129386u, 129387u,
		129388u, 129389u, 129390u, 129391u, 129392u, 129393u, 129394u, 129395u, 129396u, 129397u,
		129398u, 129399u, 129400u, 129401u, 129402u, 129403u, 129404u, 129405u, 129406u, 129407u,
		129408u, 129409u, 129410u, 129411u, 129412u, 129413u, 129414u, 129415u, 129416u, 129417u,
		129418u, 129419u, 129420u, 129421u, 129422u, 129423u, 129424u, 129425u, 129426u, 129427u,
		129428u, 129429u, 129430u, 129431u, 129432u, 129433u, 129434u, 129435u, 129436u, 129437u,
		129438u, 129439u, 129440u, 129441u, 129442u, 129443u, 129444u, 129445u, 129446u, 129447u,
		129448u, 129449u, 129450u, 129451u, 129452u, 129453u, 129454u, 129455u, 129456u, 129457u,
		129458u, 129459u, 129460u, 129461u, 129462u, 129463u, 129464u, 129465u, 129466u, 129467u,
		129468u, 129469u, 129470u, 129471u, 129472u, 129473u, 129474u, 129475u, 129476u, 129477u,
		129478u, 129479u, 129480u, 129481u, 129482u, 129483u, 129484u, 129485u, 129486u, 129487u,
		129488u, 129489u, 129490u, 129491u, 129492u, 129493u, 129494u, 129495u, 129496u, 129497u,
		129498u, 129499u, 129500u, 129501u, 129502u, 129503u, 129504u, 129505u, 129506u, 129507u,
		129508u, 129509u, 129510u, 129511u, 129512u, 129513u, 129514u, 129515u, 129516u, 129517u,
		129518u, 129519u, 129520u, 129521u, 129522u, 129523u, 129524u, 129525u, 129526u, 129527u,
		129528u, 129529u, 129530u, 129531u, 129532u, 129533u, 129534u, 129535u, 129648u, 129649u,
		129650u, 129651u, 129652u, 129653u, 129654u, 129655u, 129656u, 129657u, 129658u, 129659u,
		129660u, 129664u, 129665u, 129666u, 129667u, 129668u, 129669u, 129670u, 129671u, 129672u,
		129673u, 129679u, 129680u, 129681u, 129682u, 129683u, 129684u, 129685u, 129686u, 129687u,
		129688u, 129689u, 129690u, 129691u, 129692u, 129693u, 129694u, 129695u, 129696u, 129697u,
		129698u, 129699u, 129700u, 129701u, 129702u, 129703u, 129704u, 129705u, 129706u, 129707u,
		129708u, 129709u, 129710u, 129711u, 129712u, 129713u, 129714u, 129715u, 129716u, 129717u,
		129718u, 129719u, 129720u, 129721u, 129722u, 129723u, 129724u, 129725u, 129726u, 129727u,
		129728u, 129729u, 129730u, 129731u, 129732u, 129733u, 129734u, 129742u, 129743u, 129744u,
		129745u, 129746u, 129747u, 129748u, 129749u, 129750u, 129751u, 129752u, 129753u, 129754u,
		129755u, 129756u, 129759u, 129760u, 129761u, 129762u, 129763u, 129764u, 129765u, 129766u,
		129767u, 129768u, 129769u, 129776u, 129777u, 129778u, 129779u, 129780u, 129781u, 129782u,
		129783u, 129784u
	});

	public static bool Approximately(float a, float b)
	{
		return b - 0.0001f < a && a < b + 0.0001f;
	}

	public static Color32 HexCharsToColor(char[] hexChars, int startIndex, int tagCount)
	{
		switch (tagCount)
		{
		case 4:
		{
			byte r4 = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 1]));
			byte g4 = (byte)(HexToInt(hexChars[startIndex + 2]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte b4 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 3]));
			return new Color32(r4, g4, b4, byte.MaxValue);
		}
		case 5:
		{
			byte r3 = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 1]));
			byte g3 = (byte)(HexToInt(hexChars[startIndex + 2]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte b3 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 3]));
			byte a2 = (byte)(HexToInt(hexChars[startIndex + 4]) * 16 + HexToInt(hexChars[startIndex + 4]));
			return new Color32(r3, g3, b3, a2);
		}
		case 7:
		{
			byte r2 = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte g2 = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
			byte b2 = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
			return new Color32(r2, g2, b2, byte.MaxValue);
		}
		case 9:
		{
			byte r = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
			byte g = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
			byte b = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
			byte a = (byte)(HexToInt(hexChars[startIndex + 7]) * 16 + HexToInt(hexChars[startIndex + 8]));
			return new Color32(r, g, b, a);
		}
		default:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	public static uint HexToInt(char hex)
	{
		switch (hex)
		{
		case '0':
			return 0u;
		case '1':
			return 1u;
		case '2':
			return 2u;
		case '3':
			return 3u;
		case '4':
			return 4u;
		case '5':
			return 5u;
		case '6':
			return 6u;
		case '7':
			return 7u;
		case '8':
			return 8u;
		case '9':
			return 9u;
		case 'A':
		case 'a':
			return 10u;
		case 'B':
		case 'b':
			return 11u;
		case 'C':
		case 'c':
			return 12u;
		case 'D':
		case 'd':
			return 13u;
		case 'E':
		case 'e':
			return 14u;
		case 'F':
		case 'f':
			return 15u;
		default:
			return 15u;
		}
	}

	public static float ConvertToFloat(char[] chars, int startIndex, int length)
	{
		int lastIndex;
		return ConvertToFloat(chars, startIndex, length, out lastIndex);
	}

	public static float ConvertToFloat(char[] chars, int startIndex, int length, out int lastIndex)
	{
		if (startIndex == 0)
		{
			lastIndex = 0;
			return -32767f;
		}
		int num = startIndex + length;
		bool flag = true;
		float num2 = 0f;
		int num3 = 1;
		if (chars[startIndex] == '+')
		{
			num3 = 1;
			startIndex++;
		}
		else if (chars[startIndex] == '-')
		{
			num3 = -1;
			startIndex++;
		}
		float num4 = 0f;
		for (int i = startIndex; i < num; i++)
		{
			uint num5 = chars[i];
			if ((num5 >= 48 && num5 <= 57) || num5 == 46)
			{
				if (num5 == 46)
				{
					flag = false;
					num2 = 0.1f;
				}
				else if (flag)
				{
					num4 = num4 * 10f + (float)((num5 - 48) * num3);
				}
				else
				{
					num4 += (float)(num5 - 48) * num2 * (float)num3;
					num2 *= 0.1f;
				}
			}
			else if (num5 == 44)
			{
				if (i + 1 < num && chars[i + 1] == ' ')
				{
					lastIndex = i + 1;
				}
				else
				{
					lastIndex = i;
				}
				return num4;
			}
		}
		lastIndex = num;
		return num4;
	}

	public static void ResizeInternalArray<T>(ref T[] array)
	{
		int newSize = Mathf.NextPowerOfTwo(array.Length + 1);
		Array.Resize(ref array, newSize);
	}

	public static void ResizeInternalArray<T>(ref T[] array, int size)
	{
		size = Mathf.NextPowerOfTwo(size + 1);
		Array.Resize(ref array, size);
	}

	private static bool IsTagName(ref string text, string tag, int index)
	{
		if (text.Length < index + tag.Length)
		{
			return false;
		}
		for (int i = 0; i < tag.Length; i++)
		{
			if (TextUtilities.ToUpperFast(text[index + i]) != tag[i])
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsTagName(ref int[] text, string tag, int index)
	{
		if (text.Length < index + tag.Length)
		{
			return false;
		}
		for (int i = 0; i < tag.Length; i++)
		{
			if (TextUtilities.ToUpperFast((char)text[index + i]) != tag[i])
			{
				return false;
			}
		}
		return true;
	}

	internal static void InsertOpeningTextStyle(TextStyle style, ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		if (style != null)
		{
			textStyleStackDepth++;
			textStyleStacks[textStyleStackDepth].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
			textStyleStackDepth--;
		}
	}

	internal static void InsertClosingTextStyle(TextStyle style, ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		if (style != null)
		{
			textStyleStackDepth++;
			textStyleStacks[textStyleStackDepth].Push(style.hashCode);
			uint[] styleClosingTagArray = style.styleClosingTagArray;
			InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
			textStyleStackDepth--;
		}
	}

	public static bool ReplaceOpeningStyleTag(ref TextBackingContainer sourceText, int srcIndex, out int srcOffset, ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		int styleHashCode = GetStyleHashCode(ref sourceText, srcIndex + 7, out srcOffset);
		TextStyle style = GetStyle(generationSettings, styleHashCode);
		if (style == null || srcOffset == 0)
		{
			return false;
		}
		textStyleStackDepth++;
		textStyleStacks[textStyleStackDepth].Push(style.hashCode);
		uint[] styleOpeningTagArray = style.styleOpeningTagArray;
		InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
		textStyleStackDepth--;
		return true;
	}

	public static void ReplaceOpeningStyleTag(ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		int hashCode = textStyleStacks[textStyleStackDepth + 1].Pop();
		TextStyle style = GetStyle(generationSettings, hashCode);
		if (style != null)
		{
			textStyleStackDepth++;
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
			textStyleStackDepth--;
		}
	}

	private static bool ReplaceOpeningStyleTag(ref uint[] sourceText, int srcIndex, out int srcOffset, ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		int styleHashCode = GetStyleHashCode(ref sourceText, srcIndex + 7, out srcOffset);
		TextStyle style = GetStyle(generationSettings, styleHashCode);
		if (style == null || srcOffset == 0)
		{
			return false;
		}
		textStyleStackDepth++;
		textStyleStacks[textStyleStackDepth].Push(style.hashCode);
		uint[] styleOpeningTagArray = style.styleOpeningTagArray;
		InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
		textStyleStackDepth--;
		return true;
	}

	public static void ReplaceClosingStyleTag(ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		int hashCode = textStyleStacks[textStyleStackDepth + 1].Pop();
		TextStyle style = GetStyle(generationSettings, hashCode);
		if (style != null)
		{
			textStyleStackDepth++;
			uint[] styleClosingTagArray = style.styleClosingTagArray;
			InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
			textStyleStackDepth--;
		}
	}

	internal static void InsertOpeningStyleTag(TextStyle style, ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		if (style != null)
		{
			textStyleStacks[0].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
			textStyleStackDepth = 0;
		}
	}

	internal static void InsertClosingStyleTag(ref TextProcessingElement[] charBuffer, ref int writeIndex, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		int hashCode = textStyleStacks[0].Pop();
		TextStyle style = GetStyle(generationSettings, hashCode);
		uint[] styleClosingTagArray = style.styleClosingTagArray;
		InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
		textStyleStackDepth = 0;
	}

	private static void InsertTextStyleInTextProcessingArray(ref TextProcessingElement[] charBuffer, ref int writeIndex, uint[] styleDefinition, ref int textStyleStackDepth, ref TextProcessingStack<int>[] textStyleStacks, ref TextGenerationSettings generationSettings)
	{
		bool flag = false;
		int num = styleDefinition.Length;
		if (writeIndex + num >= charBuffer.Length)
		{
			ResizeInternalArray(ref charBuffer, writeIndex + num);
		}
		for (int i = 0; i < num; i++)
		{
			uint num2 = styleDefinition[i];
			if (num2 == 92 && i + 1 < num)
			{
				switch (styleDefinition[i + 1])
				{
				case 92u:
					i++;
					break;
				case 110u:
					num2 = 10u;
					i++;
					break;
				case 117u:
					if (i + 5 < num)
					{
						num2 = GetUTF16(styleDefinition, i + 2);
						i += 5;
					}
					break;
				case 85u:
					if (i + 9 < num)
					{
						num2 = GetUTF32(styleDefinition, i + 2);
						i += 9;
					}
					break;
				}
			}
			if (num2 == 60)
			{
				switch ((MarkupTag)GetMarkupTagHashCode(styleDefinition, i + 1))
				{
				case MarkupTag.NO_PARSE:
					flag = true;
					break;
				case MarkupTag.SLASH_NO_PARSE:
					flag = false;
					break;
				case MarkupTag.BR:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 10u;
					writeIndex++;
					i += 3;
					continue;
				case MarkupTag.CR:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 13u;
					writeIndex++;
					i += 3;
					continue;
				case MarkupTag.NBSP:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 160u;
					writeIndex++;
					i += 5;
					continue;
				case MarkupTag.ZWSP:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 8203u;
					writeIndex++;
					i += 5;
					continue;
				case MarkupTag.ZWJ:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 8205u;
					writeIndex++;
					i += 4;
					continue;
				case MarkupTag.SHY:
					if (flag)
					{
						break;
					}
					charBuffer[writeIndex].unicode = 173u;
					writeIndex++;
					i += 4;
					continue;
				case MarkupTag.STYLE:
				{
					if (flag || !ReplaceOpeningStyleTag(ref styleDefinition, i, out var srcOffset, ref charBuffer, ref writeIndex, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings))
					{
						break;
					}
					int num3 = num - srcOffset;
					i = srcOffset;
					if (writeIndex + num3 >= charBuffer.Length)
					{
						ResizeInternalArray(ref charBuffer, writeIndex + num3);
					}
					continue;
				}
				case MarkupTag.SLASH_STYLE:
					if (flag)
					{
						break;
					}
					ReplaceClosingStyleTag(ref charBuffer, ref writeIndex, ref textStyleStackDepth, ref textStyleStacks, ref generationSettings);
					i += 7;
					continue;
				}
			}
			charBuffer[writeIndex].unicode = num2;
			writeIndex++;
		}
	}

	public static TextStyle GetStyle(TextGenerationSettings generationSetting, int hashCode)
	{
		TextStyle textStyle = null;
		TextStyleSheet textStyleSheet = null;
		if (textStyleSheet != null)
		{
			textStyle = textStyleSheet.GetStyle(hashCode);
			if (textStyle != null)
			{
				return textStyle;
			}
		}
		textStyleSheet = generationSetting.textSettings.defaultStyleSheet;
		if (textStyleSheet != null)
		{
			textStyle = textStyleSheet.GetStyle(hashCode);
		}
		return textStyle;
	}

	public static int GetStyleHashCode(ref uint[] text, int index, out int closeIndex)
	{
		int num = 0;
		closeIndex = 0;
		for (int i = index; i < text.Length; i++)
		{
			if (text[i] != 34)
			{
				if (text[i] == 62)
				{
					closeIndex = i;
					break;
				}
				num = ((num << 5) + num) ^ ToUpperASCIIFast((char)text[i]);
			}
		}
		return num;
	}

	public static int GetStyleHashCode(ref TextBackingContainer text, int index, out int closeIndex)
	{
		int num = 0;
		closeIndex = 0;
		for (int i = index; i < text.Capacity; i++)
		{
			if (text[i] != 34)
			{
				if (text[i] == 62)
				{
					closeIndex = i;
					break;
				}
				num = ((num << 5) + num) ^ ToUpperASCIIFast((char)text[i]);
			}
		}
		return num;
	}

	public static uint GetUTF16(uint[] text, int i)
	{
		uint num = 0u;
		num += HexToInt((char)text[i]) << 12;
		num += HexToInt((char)text[i + 1]) << 8;
		num += HexToInt((char)text[i + 2]) << 4;
		return num + HexToInt((char)text[i + 3]);
	}

	public static uint GetUTF16(TextBackingContainer text, int i)
	{
		uint num = 0u;
		num += HexToInt((char)text[i]) << 12;
		num += HexToInt((char)text[i + 1]) << 8;
		num += HexToInt((char)text[i + 2]) << 4;
		return num + HexToInt((char)text[i + 3]);
	}

	public static uint GetUTF32(uint[] text, int i)
	{
		uint num = 0u;
		num += HexToInt((char)text[i]) << 28;
		num += HexToInt((char)text[i + 1]) << 24;
		num += HexToInt((char)text[i + 2]) << 20;
		num += HexToInt((char)text[i + 3]) << 16;
		num += HexToInt((char)text[i + 4]) << 12;
		num += HexToInt((char)text[i + 5]) << 8;
		num += HexToInt((char)text[i + 6]) << 4;
		return num + HexToInt((char)text[i + 7]);
	}

	public static uint GetUTF32(TextBackingContainer text, int i)
	{
		uint num = 0u;
		num += HexToInt((char)text[i]) << 28;
		num += HexToInt((char)text[i + 1]) << 24;
		num += HexToInt((char)text[i + 2]) << 20;
		num += HexToInt((char)text[i + 3]) << 16;
		num += HexToInt((char)text[i + 4]) << 12;
		num += HexToInt((char)text[i + 5]) << 8;
		num += HexToInt((char)text[i + 6]) << 4;
		return num + HexToInt((char)text[i + 7]);
	}

	public static void FillCharacterVertexBuffers(int i, bool convertToLinearSpace, TextGenerationSettings generationSettings, TextInfo textInfo, bool needToRound)
	{
		int materialReferenceIndex = textInfo.textElementInfo[i].materialReferenceIndex;
		int vertexCount = textInfo.meshInfo[materialReferenceIndex].vertexCount;
		if (vertexCount >= textInfo.meshInfo[materialReferenceIndex].vertexBufferSize)
		{
			textInfo.meshInfo[materialReferenceIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((vertexCount + 4) / 4), generationSettings.isIMGUI);
		}
		if (textInfo.meshInfo[materialReferenceIndex].vertexData.Length >= vertexCount + 4)
		{
			TextElementInfo[] textElementInfo = textInfo.textElementInfo;
			textInfo.textElementInfo[i].vertexIndex = vertexCount;
			Vector3 vector = default(Vector3);
			vector.x = 0f;
			vector.y = generationSettings.screenRect.height;
			if (needToRound)
			{
				vector.y = Mathf.Round(vector.y);
			}
			vector.z = 0f;
			Vector3 position = textElementInfo[i].vertexBottomLeft.position;
			position.y *= -1f;
			textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].position = position + vector;
			position = textElementInfo[i].vertexTopLeft.position;
			position.y *= -1f;
			textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].position = position + vector;
			position = textElementInfo[i].vertexTopRight.position;
			position.y *= -1f;
			textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].position = position + vector;
			position = textElementInfo[i].vertexBottomRight.position;
			position.y *= -1f;
			textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].position = position + vector;
			textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].uv0 = textElementInfo[i].vertexBottomLeft.uv;
			textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].uv0 = textElementInfo[i].vertexTopLeft.uv;
			textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].uv0 = textElementInfo[i].vertexTopRight.uv;
			textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].uv0 = textElementInfo[i].vertexBottomRight.uv;
			textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].uv2 = textElementInfo[i].vertexBottomLeft.uv2;
			textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].uv2 = textElementInfo[i].vertexTopLeft.uv2;
			textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].uv2 = textElementInfo[i].vertexTopRight.uv2;
			textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].uv2 = textElementInfo[i].vertexBottomRight.uv2;
			textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexBottomLeft.color) : textElementInfo[i].vertexBottomLeft.color);
			textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexTopLeft.color) : textElementInfo[i].vertexTopLeft.color);
			textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexTopRight.color) : textElementInfo[i].vertexTopRight.color);
			textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexBottomRight.color) : textElementInfo[i].vertexBottomRight.color);
			textInfo.meshInfo[materialReferenceIndex].vertexCount = vertexCount + 4;
		}
	}

	public static void FillSpriteVertexBuffers(int i, bool convertToLinearSpace, TextGenerationSettings generationSettings, TextInfo textInfo)
	{
		int materialReferenceIndex = textInfo.textElementInfo[i].materialReferenceIndex;
		int vertexCount = textInfo.meshInfo[materialReferenceIndex].vertexCount;
		textInfo.meshInfo[materialReferenceIndex].applySDF = false;
		TextElementInfo[] textElementInfo = textInfo.textElementInfo;
		textInfo.textElementInfo[i].vertexIndex = vertexCount;
		Vector3 vector = default(Vector3);
		vector.x = 0f;
		vector.y = generationSettings.screenRect.height;
		vector.z = 0f;
		Vector3 position = textElementInfo[i].vertexBottomLeft.position;
		position.y *= -1f;
		textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].position = position + vector;
		position = textElementInfo[i].vertexTopLeft.position;
		position.y *= -1f;
		textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].position = position + vector;
		position = textElementInfo[i].vertexTopRight.position;
		position.y *= -1f;
		textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].position = position + vector;
		position = textElementInfo[i].vertexBottomRight.position;
		position.y *= -1f;
		textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].position = position + vector;
		textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].uv0 = textElementInfo[i].vertexBottomLeft.uv;
		textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].uv0 = textElementInfo[i].vertexTopLeft.uv;
		textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].uv0 = textElementInfo[i].vertexTopRight.uv;
		textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].uv0 = textElementInfo[i].vertexBottomRight.uv;
		textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].uv2 = textElementInfo[i].vertexBottomLeft.uv2;
		textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].uv2 = textElementInfo[i].vertexTopLeft.uv2;
		textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].uv2 = textElementInfo[i].vertexTopRight.uv2;
		textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].uv2 = textElementInfo[i].vertexBottomRight.uv2;
		textInfo.meshInfo[materialReferenceIndex].vertexData[vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexBottomLeft.color) : textElementInfo[i].vertexBottomLeft.color);
		textInfo.meshInfo[materialReferenceIndex].vertexData[1 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexTopLeft.color) : textElementInfo[i].vertexTopLeft.color);
		textInfo.meshInfo[materialReferenceIndex].vertexData[2 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexTopRight.color) : textElementInfo[i].vertexTopRight.color);
		textInfo.meshInfo[materialReferenceIndex].vertexData[3 + vertexCount].color = (convertToLinearSpace ? GammaToLinear(textElementInfo[i].vertexBottomRight.color) : textElementInfo[i].vertexBottomRight.color);
		textInfo.meshInfo[materialReferenceIndex].vertexCount = vertexCount + 4;
	}

	public static void AdjustLineOffset(int startIndex, int endIndex, float offset, TextInfo textInfo)
	{
		Vector3 vector = new Vector3(0f, offset, 0f);
		for (int i = startIndex; i <= endIndex; i++)
		{
			textInfo.textElementInfo[i].bottomLeft -= vector;
			textInfo.textElementInfo[i].topLeft -= vector;
			textInfo.textElementInfo[i].topRight -= vector;
			textInfo.textElementInfo[i].bottomRight -= vector;
			textInfo.textElementInfo[i].ascender -= vector.y;
			textInfo.textElementInfo[i].baseLine -= vector.y;
			textInfo.textElementInfo[i].descender -= vector.y;
			if (textInfo.textElementInfo[i].isVisible)
			{
				textInfo.textElementInfo[i].vertexBottomLeft.position -= vector;
				textInfo.textElementInfo[i].vertexTopLeft.position -= vector;
				textInfo.textElementInfo[i].vertexTopRight.position -= vector;
				textInfo.textElementInfo[i].vertexBottomRight.position -= vector;
			}
		}
	}

	public static void ResizeLineExtents(int size, TextInfo textInfo)
	{
		size = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size + 1));
		LineInfo[] array = new LineInfo[size];
		for (int i = 0; i < size; i++)
		{
			if (i < textInfo.lineInfo.Length)
			{
				array[i] = textInfo.lineInfo[i];
				continue;
			}
			array[i].lineExtents.min = largePositiveVector2;
			array[i].lineExtents.max = largeNegativeVector2;
			array[i].ascender = -32767f;
			array[i].descender = 32767f;
		}
		textInfo.lineInfo = array;
	}

	public static FontStyles LegacyStyleToNewStyle(FontStyle fontStyle)
	{
		return fontStyle switch
		{
			FontStyle.Bold => FontStyles.Bold, 
			FontStyle.Italic => FontStyles.Italic, 
			FontStyle.BoldAndItalic => FontStyles.Bold | FontStyles.Italic, 
			_ => FontStyles.Normal, 
		};
	}

	public static TextAlignment LegacyAlignmentToNewAlignment(TextAnchor anchor)
	{
		return anchor switch
		{
			TextAnchor.UpperLeft => TextAlignment.TopLeft, 
			TextAnchor.UpperCenter => TextAlignment.TopCenter, 
			TextAnchor.UpperRight => TextAlignment.TopRight, 
			TextAnchor.MiddleLeft => TextAlignment.MiddleLeft, 
			TextAnchor.MiddleCenter => TextAlignment.MiddleCenter, 
			TextAnchor.MiddleRight => TextAlignment.MiddleRight, 
			TextAnchor.LowerLeft => TextAlignment.BottomLeft, 
			TextAnchor.LowerCenter => TextAlignment.BottomCenter, 
			TextAnchor.LowerRight => TextAlignment.BottomRight, 
			_ => TextAlignment.TopLeft, 
		};
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static UnityEngine.TextCore.HorizontalAlignment GetHorizontalAlignment(TextAnchor anchor)
	{
		switch (anchor)
		{
		case TextAnchor.UpperLeft:
		case TextAnchor.MiddleLeft:
		case TextAnchor.LowerLeft:
			return UnityEngine.TextCore.HorizontalAlignment.Left;
		case TextAnchor.UpperCenter:
		case TextAnchor.MiddleCenter:
		case TextAnchor.LowerCenter:
			return UnityEngine.TextCore.HorizontalAlignment.Center;
		case TextAnchor.UpperRight:
		case TextAnchor.MiddleRight:
		case TextAnchor.LowerRight:
			return UnityEngine.TextCore.HorizontalAlignment.Right;
		default:
			return UnityEngine.TextCore.HorizontalAlignment.Left;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static UnityEngine.TextCore.VerticalAlignment GetVerticalAlignment(TextAnchor anchor)
	{
		switch (anchor)
		{
		case TextAnchor.LowerLeft:
		case TextAnchor.LowerCenter:
		case TextAnchor.LowerRight:
			return UnityEngine.TextCore.VerticalAlignment.Bottom;
		case TextAnchor.MiddleLeft:
		case TextAnchor.MiddleCenter:
		case TextAnchor.MiddleRight:
			return UnityEngine.TextCore.VerticalAlignment.Middle;
		case TextAnchor.UpperLeft:
		case TextAnchor.UpperCenter:
		case TextAnchor.UpperRight:
			return UnityEngine.TextCore.VerticalAlignment.Top;
		default:
			return UnityEngine.TextCore.VerticalAlignment.Top;
		}
	}

	public static uint ConvertToUTF32(uint highSurrogate, uint lowSurrogate)
	{
		return (highSurrogate - 55296) * 1024 + (lowSurrogate - 56320 + 65536);
	}

	public static int GetMarkupTagHashCode(TextBackingContainer styleDefinition, int readIndex)
	{
		int num = 0;
		int num2 = readIndex + 16;
		int capacity = styleDefinition.Capacity;
		while (readIndex < num2 && readIndex < capacity)
		{
			uint num3 = styleDefinition[readIndex];
			if (num3 == 62 || num3 == 61 || num3 == 32)
			{
				return num;
			}
			num = ((num << 5) + num) ^ (int)ToUpperASCIIFast(num3);
			readIndex++;
		}
		return num;
	}

	public static int GetMarkupTagHashCode(uint[] styleDefinition, int readIndex)
	{
		int num = 0;
		int num2 = readIndex + 16;
		int num3 = styleDefinition.Length;
		while (readIndex < num2 && readIndex < num3)
		{
			uint num4 = styleDefinition[readIndex];
			if (num4 == 62 || num4 == 61 || num4 == 32)
			{
				return num;
			}
			num = ((num << 5) + num) ^ (int)ToUpperASCIIFast(num4);
			readIndex++;
		}
		return num;
	}

	public static char ToUpperASCIIFast(char c)
	{
		if (c > "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-".Length - 1)
		{
			return c;
		}
		return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[c];
	}

	public static uint ToUpperASCIIFast(uint c)
	{
		if (c > "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-".Length - 1)
		{
			return c;
		}
		return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[(int)c];
	}

	public static char ToUpperFast(char c)
	{
		if (c > "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-".Length - 1)
		{
			return c;
		}
		return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[c];
	}

	public static int GetAttributeParameters(char[] chars, int startIndex, int length, ref float[] parameters)
	{
		int lastIndex = startIndex;
		int num = 0;
		while (lastIndex < startIndex + length)
		{
			parameters[num] = ConvertToFloat(chars, startIndex, length, out lastIndex);
			length -= lastIndex - startIndex + 1;
			startIndex = lastIndex + 1;
			num++;
		}
		return num;
	}

	public static bool IsBitmapRendering(GlyphRenderMode glyphRenderMode)
	{
		return glyphRenderMode == GlyphRenderMode.RASTER || glyphRenderMode == GlyphRenderMode.RASTER_HINTED || glyphRenderMode == GlyphRenderMode.SMOOTH || glyphRenderMode == GlyphRenderMode.SMOOTH_HINTED;
	}

	public static bool IsBaseGlyph(uint c)
	{
		return (c < 768 || c > 879) && (c < 6832 || c > 6911) && (c < 7616 || c > 7679) && (c < 8400 || c > 8447) && (c < 65056 || c > 65071) && c != 3633 && (c < 3636 || c > 3642) && (c < 3655 || c > 3662) && (c < 1425 || c > 1469) && c != 1471 && (c < 1473 || c > 1474) && (c < 1476 || c > 1477) && c != 1479 && (c < 1552 || c > 1562) && (c < 1611 || c > 1631) && c != 1648 && (c < 1750 || c > 1756) && (c < 1759 || c > 1764) && (c < 1767 || c > 1768) && (c < 1770 || c > 1773) && (c < 2259 || c > 2273) && (c < 2275 || c > 2303) && (c < 64434 || c > 64449);
	}

	public static Color MinAlpha(this Color c1, Color c2)
	{
		float a = ((c1.a < c2.a) ? c1.a : c2.a);
		return new Color(c1.r, c1.g, c1.b, a);
	}

	internal static Color32 GammaToLinear(Color32 c)
	{
		return new Color32(GammaToLinear(c.r), GammaToLinear(c.g), GammaToLinear(c.b), c.a);
	}

	private static byte GammaToLinear(byte value)
	{
		float num = (float)(int)value / 255f;
		if (num <= 0.04045f)
		{
			return (byte)(num / 12.92f * 255f);
		}
		if (num < 1f)
		{
			return (byte)(Mathf.Pow((num + 0.055f) / 1.055f, 2.4f) * 255f);
		}
		if (num == 1f)
		{
			return byte.MaxValue;
		}
		return (byte)(Mathf.Pow(num, 2.2f) * 255f);
	}

	public static bool IsValidUTF16(TextBackingContainer text, int index)
	{
		for (int i = 0; i < 4; i++)
		{
			uint num = text[index + i];
			if ((num < 48 || num > 57) && (num < 97 || num > 102) && (num < 65 || num > 70))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsValidUTF32(TextBackingContainer text, int index)
	{
		for (int i = 0; i < 8; i++)
		{
			uint num = text[index + i];
			if ((num < 48 || num > 57) && (num < 97 || num > 102) && (num < 65 || num > 70))
			{
				return false;
			}
		}
		return true;
	}

	internal static bool IsEmoji(uint c)
	{
		return k_EmojiLookup.Contains(c);
	}

	internal static bool IsEmojiPresentationForm(uint c)
	{
		return k_EmojiPresentationFormLookup.Contains(c);
	}

	internal static bool IsHangul(uint c)
	{
		return (c >= 4352 && c <= 4607) || (c >= 43360 && c <= 43391) || (c >= 55216 && c <= 55295) || (c >= 12592 && c <= 12687) || (c >= 65440 && c <= 65500) || (c >= 44032 && c <= 55215);
	}

	internal static bool IsCJK(uint c)
	{
		return (c >= 12288 && c <= 12351) || (c >= 94176 && c <= 5887) || (c >= 12544 && c <= 12591) || (c >= 12704 && c <= 12735) || (c >= 19968 && c <= 40959) || (c >= 13312 && c <= 19903) || (c >= 131072 && c <= 173791) || (c >= 173824 && c <= 177983) || (c >= 177984 && c <= 178207) || (c >= 178208 && c <= 183983) || (c >= 183984 && c <= 191456) || (c >= 196608 && c <= 201546) || (c >= 63744 && c <= 64255) || (c >= 194560 && c <= 195103) || (c >= 12032 && c <= 12255) || (c >= 11904 && c <= 12031) || (c >= 12736 && c <= 12783) || (c >= 12272 && c <= 12287) || (c >= 12352 && c <= 12447) || (c >= 110848 && c <= 110895) || (c >= 110576 && c <= 110591) || (c >= 110592 && c <= 110847) || (c >= 110896 && c <= 110959) || (c >= 12688 && c <= 12703) || (c >= 12448 && c <= 12543) || (c >= 12784 && c <= 12799) || (c >= 65381 && c <= 65439);
	}
}
