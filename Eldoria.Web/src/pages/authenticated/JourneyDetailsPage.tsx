import React, { useEffect, useState } from 'react'
import AppPage from '../../components/Layout/AppPage'
import PageContent from '../../components/Layout/PageContent'
import AppButton, { AppButtonSize, AppButtonVariant } from '../../components/buttons/AppButton'
import { useNavigate, useParams } from 'react-router'
import { JourneyDto } from '../../types/journey'
import { BASE_URL, useApi } from '../../hooks/useApi'
import { ToastType, useToast } from '../../providers/ToastProvider'
import Title, { TitleColor } from '../../components/typography/Title'
import { TextColor } from '../../components/typography/Text'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPlus } from '@fortawesome/free-solid-svg-icons'
import clsx from 'clsx'
import { useModalRouter } from '../../providers/ModalRouterProvider'
import JourneyPlayersModal from '../../components/Modal/JourneyPlayersModal'

const JourneyDetailsPage = () => {    
    const {get} = useApi();
    const modalRouter = useModalRouter();
    const { showToast } = useToast();
    const { id } = useParams();
    const journeyId = Number(id);

    const [journey, setJourney] = useState<JourneyDto>();

    const getJourney = async () => {
        try {
            const journey = await get(`${BASE_URL}/Journey/${journeyId}`);
            setJourney(journey);
        } catch (err) {
            showToast("Error:", "Unable to get journey details.", ToastType.error, 3000);            
        }
    }

    useEffect(() => {
        getJourney();
    }, []);

    const openPlayersModal = () => {
        modalRouter.push(<JourneyPlayersModal />)
    }
    
  return (
        <AppPage backgroundImage='/Stone_Background.png' pane={        <AppButton
          onClick={openPlayersModal}
          title={"Players"}
          variant={AppButtonVariant.primary}
          size={AppButtonSize.md}
          noRounded
          className="absolute bottom-20 -left-11 rounded-t-2xl rotate-90 z-20"
        />}>
            <PageContent useBackButton noTopMargin noCentering className='mt-2'>
                <div className='flex'>
                    <div className='flex w-[40%] justify-center'>
                        <img className='w-[80%] self-center rounded-3xl' src={journey?.photoUrl} />
                        
                    </div>
                    <div className='flex flex-col w-[60%]'>
                        {/* Scenes Section */}
                        <div className='flex w-full border-b-4 border-black items-center'>
                            <Title className='flex-1' color={TitleColor.default}>Scenes</Title>
                            <AppButton title={'Add Scene'} variant={AppButtonVariant.go} size={AppButtonSize.md} 
                            rightIcon={<FontAwesomeIcon icon={faPlus} size='lg' className='ml-2'/>}/>
                        </div>
                    </div>                    
                </div>        
            </PageContent>
        </AppPage>      
  )
}

export default JourneyDetailsPage