import React, { useState } from 'react'

interface InputValues{
    characterId?: number;
    orderNum?: number;
    readingText?: string;
    isNarrator?: boolean;
}

interface InputErrors{
    characterId?: string;
    orderNum?: string;
    readingText?: string;
    isNarrator?: string;    
}

interface CreatePageSectionProps{
    pageDialogId: number;
    onRefreshRequest: () => void;
}

const CreatePageSection = ({
    pageDialogId,
    onRefreshRequest
}: CreatePageSectionProps) => {
    const [isCreateOpen, setIsCreateOpen] = useState(false);

    const [inputValues, setInputValue] = useState<InputValues>({
        characterId: undefined,
        orderNum: undefined,
        readingText: undefined,
        isNarrator: undefined
    })

    const [inputErrors, setInputErrors] = useState<InputErrors>({
        characterId: undefined,
        orderNum: undefined,
        readingText: undefined,
        isNarrator: undefined
    })

    const save = () => {

    }

    const validate = () => {

    }

  return (
    <div>CreatePageSection</div>
  )
}

export default CreatePageSection