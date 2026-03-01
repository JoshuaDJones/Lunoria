import { useState } from 'react';
import { useModalRouter } from '../../providers/ModalRouterProvider';
import FileInput from '../inputs/FileInput';
import AppModal from './AppModal'
import AppButton, { AppButtonSize, AppButtonVariant } from '../buttons/AppButton';

interface PhotoPickerModalProps {

}

const PhotoPickerModal = ({

}: PhotoPickerModalProps) => {
    const modalRouter = useModalRouter();

    const handleContentClick = (e: React.MouseEvent) => {
        e.stopPropagation();
    };

    const [selectedPhoto, setSelectedPhoto] = useState<File | undefined>(undefined);

  return (
    <AppModal onBackgroundClose={modalRouter.pop} centerContent backgroundOverride="bg-stone-500/50">
      <div className="flex flex-col p-5 rounded-2xl bg-black" onClick={handleContentClick}>
        
        <FileInput title="Select Photo" onFileSelect={(file) => setSelectedPhoto(file)} />
            {selectedPhoto && (
          <img
            src={URL.createObjectURL(selectedPhoto)}
            className="w-[80px] rounded-lg mt-4"
          />
        )}
        <AppButton title="Save" disabled={!selectedPhoto} onClick={modalRouter.pop} className="mt-5 self-center" variant={!selectedPhoto ? AppButtonVariant.disabled : AppButtonVariant.primary} size={AppButtonSize.sm} />
      </div>
    </AppModal>
  )
}

export default PhotoPickerModal