import React from 'react'
import LeftModalContent from './LeftModalContent'
import Modal from './Modal'
import AppModal from './AppModal'
import { useModalRouter } from '../../providers/ModalRouterProvider'
import AppButton, { AppButtonSize, AppButtonVariant } from '../buttons/AppButton'

type Props = {}

const JourneyPlayersModal = (props: Props) => {
    const modalRouter = useModalRouter();

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
        <LeftModalContent className='w-[20%]' title={'Players'}>
            <div className='flex-1 bg-red-100'>

            </div>
            <AppButton title={'Save'} variant={AppButtonVariant.ghost} size={AppButtonSize.md} />
        </LeftModalContent>
    </AppModal>
    
  )
}

export default JourneyPlayersModal