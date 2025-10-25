import Text from '../typography/Text';
import { DialogPageSectionDto } from '../../types/scene'

interface ViewDialogSectionProps{
    pageSection: DialogPageSectionDto;
}

const ViewDialogSection = ({
    pageSection
}: ViewDialogSectionProps) => {
  return (
    <div className='border-2 rounded-xl'>
        <Text>{pageSection.readingText}</Text>
    </div>
  )
}

export default ViewDialogSection