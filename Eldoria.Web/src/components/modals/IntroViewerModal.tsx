import { useModalRouter } from "../../providers/ModalRouterProvider";
import AppModal from "../modals/AppModal";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faClose } from "@fortawesome/free-solid-svg-icons";
import Text, { TextColor, TextSize } from "../typography/Text";
import Title, { TitleSize, TitleColor } from "../typography/Title";
import { useState } from "react";

enum IntroPage{
    prelude,
    kofi,
    rou,
    sonya,
    cael
}

interface IntroViewerModalProps {
  
}

const IntroViewerModal = ({
}: IntroViewerModalProps) => {
  const modalRouter = useModalRouter();

  const [activePage, setActivePage] = useState(IntroPage.prelude);

  const changePage = (isForward: boolean) => {
    switch(activePage){
        case IntroPage.prelude:
            if (isForward)
                setActivePage(IntroPage.kofi);            
            else 
                setActivePage(IntroPage.cael);
            break;
        case IntroPage.kofi:
            if(isForward)
                setActivePage(IntroPage.rou);
            else
                setActivePage(IntroPage.prelude);
            break;
        case IntroPage.rou:
            if(isForward)
                setActivePage(IntroPage.sonya);
            else
                setActivePage(IntroPage.kofi);
            break;
            case IntroPage.sonya:
            if(isForward)
                setActivePage(IntroPage.cael);
            else
                setActivePage(IntroPage.rou);
            break;
            case IntroPage.cael:
            if(!isForward)
                setActivePage(IntroPage.sonya);            
            else 
                setActivePage(IntroPage.prelude);
            break;
    }
  }

  const renderSection = () => {
    switch(activePage){
        case IntroPage.prelude:
            return <PreludeSection />
            case IntroPage.kofi:
            return <KofiSection />
            case IntroPage.rou:
            return <RouSection />
            case IntroPage.sonya:
            return <SonyaSection />
            case IntroPage.cael:
            return <CaelSection />
    }
  }

  return (
    <AppModal centerContent>
      <div className="flex flex-col h-full w-full bg-stone-900/80 backdrop-blur-md px-5 py-2 relative">
        <button className="mt-2 text-white self-end" onClick={() => modalRouter.pop()}>
            <FontAwesomeIcon
        icon={faClose}
        size="2x"
        className="text-white hover:text-white/50"
        />
        </button>
        
        {renderSection()}

        <div className="flex justify-between mb-3">
            <button onClick={() => changePage(false)} className="border-2 px-2 py-1 rounded-lg hover:bg-stone-500/20">
                <Text textColor={TextColor.white} size={TextSize.xl}>Prev</Text>
            </button>
            <button onClick={() => changePage(true)} className="border-2 px-2 py-1 rounded-lg hover:bg-stone-500/20">
                <Text textColor={TextColor.white} size={TextSize.xl}>Next</Text>
            </button>
        </div>
      </div>
    </AppModal>
  );
};

const PreludeSection = () => (
    <div className="flex-1 mb-3 h-full w-full">
        <div className="flex-1 flex w-full h-full gap-10 p-3">
            <div className="flex-1 flex items-center justify-center">
                <img src="/Lunoria_Map.jpg" className="flex-1 rounded-lg" />
            </div>
            <div className="flex-1 flex flex-col gap-4">                
                <Title size={TitleSize.xlarge} color={TitleColor.white}>Lunoria</Title>  

                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- A world shaped by magic.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- Aga is magic.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- There was one original Aga user (The Aga) and through his bloodline over the centuries has spread across Lunoria.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- 1/300 people have aga abilities</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- Aga is any kind of magic (fire, ice, lightning, dark, healing, earth, sun and moon)</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">- Originally only the original bloodline could access such power but over time it spread across the lands</Text>                                
            </div>
        </div>
    </div>
)

const KofiSection = () => (
<div className="flex-1 mb-3 h-full w-full">
        <div className="flex-1 flex w-full h-full gap-10 p-3">
            <div className="flex-1 flex-col">
                <div className="flex-1 overflow-auto flex items-center justify-center">
                    <img src="https://jdjstorageaccount.blob.core.windows.net/recipe-photos/e14453ad-e33d-4d49-b4ea-5245859e4a7f.png" className="rounded-lg w-[500px]" />
                </div>
                <div className="flex-1 bg-green-200">

                </div>
                
             </div>
            <div className="flex-1 flex flex-col gap-4">                
                <Title size={TitleSize.xlarge} color={TitleColor.white}>Kofi</Title>

                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Here is your character Kofi, she is a halfling who is a lighthearted, traveler/adventurer, well known and loved by many people across Lunoria.</Text>                
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She travels with her companion Tiku, a giant mountain wolf who has been by her side since Kofi’s birth in SnowBurrow.</Text>                
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Kofi is the heart of the group with her optimism and kind character that draws people to her.</Text>                
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She a rouge type character that specializes in movement and ice magic.</Text>                
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She is able to get on and off of Tiku during combat which alters her movement and melee.</Text>                
        </div>
    </div>
    </div>
)

const RouSection = () => (
<div className="flex-1 mb-3 h-full w-full">
        <div className="flex-1 flex w-full h-full gap-10 p-3">
            <div className="flex-1 flex-col">
                <div className="flex-1 overflow-auto flex items-center justify-center">
                    <img src="https://jdjstorageaccount.blob.core.windows.net/recipe-photos/34e94767-9737-4af4-ac35-6a30f91c6b10.png" className="rounded-lg w-[500px]" />
                </div>
                <div className="flex-1 bg-green-200">

                </div>
                
             </div>
            <div className="flex-1 flex flex-col gap-4">                
                <Title size={TitleSize.xlarge} color={TitleColor.white}>Rou</Title>

                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Here is your character Rou</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">she is a sarcastic, funny, loyal druid from Llianni Village in the heart of the Vireth Empire.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She has the ability to transform into a giant winged beast.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">In base form she specializes in healing and close quarters combat.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">In beast form she specializes in movement and strong magic abilities.</Text>                            
        </div>
    </div>
    </div>
)



const SonyaSection = () => (
<div className="flex-1 mb-3 h-full w-full">
        <div className="flex-1 flex w-full h-full gap-10 p-3">
            <div className="flex-1 flex-col">
                <div className="flex-1 overflow-auto flex items-center justify-center">
                    <img src="https://jdjstorageaccount.blob.core.windows.net/recipe-photos/8b4c01c8-c020-487b-8c75-08a6a34fa44e.png" className="rounded-lg w-[500px]" />
                </div>
                <div className="flex-1 bg-green-200">

                </div>
                
             </div>
            <div className="flex-1 flex flex-col gap-4">                
                <Title size={TitleSize.xlarge} color={TitleColor.white}>Sonya</Title>

                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Here is your character Sonya, she has a divine aura due to her high power scaling magic use.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She is of royal lineage in the Empire of Visskari.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She is kind and intelligent, and amongst the most powerful mages in Lunoria.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Sonya is a sorcerer glass cannon type character, that can use all types of magic abilities.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">She has high magic use with low health and melee attacks.</Text>                
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Void piercer is a ability that was taught to her by the high mage of visskari that deals a high damage output essentially a sniper rifle.</Text>
        </div>
    </div>
    </div>
)

const CaelSection = () => (
<div className="flex-1 mb-3 h-full w-full">
        <div className="flex-1 flex w-full h-full gap-10 p-3">
            <div className="flex-1 flex-col">
                <div className="flex-1 overflow-auto flex items-center justify-center">
                    <img src="https://jdjstorageaccount.blob.core.windows.net/recipe-photos/b646ae04-67cb-49d3-90b3-07211fd58c2b.png" className="rounded-lg w-[500px]" />
                </div>
                <div className="flex-1 bg-green-200">

                </div>
                
             </div>
            <div className="flex-1 flex flex-col gap-4">                
                <Title size={TitleSize.xlarge} color={TitleColor.white}>Cael</Title>

                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Here is your character Cael, he is a natural born leader.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">A stoic character with the burden of destiny.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">A lord from the Vireth Empire who bases he whole identity around being honorable and doing the right thing for his people.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Cael is a tank/warrior style character who takes alot of damage for the team.</Text>
                <Text size={TextSize.x3l} textColor={TextColor.white} className="">Delivers consistent attack damage, also has some attack magic abilities.</Text>                
        </div>
    </div>
    </div>
)

export default IntroViewerModal;
