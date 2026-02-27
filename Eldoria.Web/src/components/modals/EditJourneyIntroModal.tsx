import React from 'react'
import AppModal from './AppModal'
import AppButton, { AppButtonSize, AppButtonVariant } from '../buttons/AppButton'
import { faArrowCircleLeft } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import CloseIconButton from '../buttons/CloseIconButton';

interface EditJourneyIntroModalProps{

}

const EditJourneyIntroModal = (props: EditJourneyIntroModalProps) => {
  return (
    <AppModal centerContent>
        <div className='flex flex-col h-full w-full bg-stone-900/80 backdrop-blur-md'>
            <CloseIconButton onCloseClick={() => {}} />                
        </div>
    </AppModal>
  )
}

export default EditJourneyIntroModal