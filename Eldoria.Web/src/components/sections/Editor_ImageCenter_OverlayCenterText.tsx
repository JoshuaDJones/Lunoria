import { useEffect, useState } from 'react';
import { EditorContent, useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import { IntroPage } from '../../types/journey';
import AppButton, { AppButtonSize, AppButtonVariant } from '../buttons/AppButton';
import { useModalRouter } from '../../providers/ModalRouterProvider';
import PhotoPickerModal from '../modals/PhotoPickerModal';

enum EditorState {
    Add,
    Edit
}

interface Editor_ImageCenter_OverlayCenterTextProps {
  workingIntroPage?: IntroPage;
  onSave: (introPage: IntroPage) => void;
}

const Editor_ImageCenter_OverlayCenterText = ({
    workingIntroPage,
    onSave
}: Editor_ImageCenter_OverlayCenterTextProps) => {
    const modalRouter = useModalRouter();
    const [editorState, setEditorState] = useState<EditorState>(workingIntroPage ? EditorState.Edit : EditorState.Add);
    const [imageUrl, setImageUrl] = useState<string>(workingIntroPage && workingIntroPage.type === "ImageCenter_OverlayCenterText" ? workingIntroPage.config.imageUrl : "");
    const [content, setContent] = useState<string>(workingIntroPage && workingIntroPage.type === "ImageCenter_OverlayCenterText" ? workingIntroPage.config.content : "");

    const editor = useEditor({
        extensions: [StarterKit],
        content,
        onUpdate: ({ editor }) => {
            setContent(editor.getHTML());
        },
    });

    useEffect(() => {
        setEditorState(workingIntroPage ? EditorState.Edit : EditorState.Add);
        if(workingIntroPage && workingIntroPage.type === "ImageCenter_OverlayCenterText") {
            setImageUrl(workingIntroPage.config.imageUrl);
            setContent(workingIntroPage.config.content);
            if (editor) {
                editor.commands.setContent(workingIntroPage.config.content);
            }
        } else {
            setImageUrl("");
            setContent("");
            if (editor) {
                editor.commands.setContent("");
            }
        }
    }, [workingIntroPage, editor]);

    //Add Mode
    // - show image placeholder
    // - show add image button
    // - show add text button

    //when either of them are blicked then hide thatbutton and show either a image selector
    // or a richtext editor 

  return (
    <div className='flex flex-col gap-2'>
        <div className='flex flex-col border-2 border-white w-full h-[600px] relative'>
        {/* <div className="flex flex-1 bg-gradient-to-br from-stone-400 to-stone-800">
            <div className='absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 flex gap-2'>
                <AppButton title="Add Image" onClick={() => modalRouter.push(<PhotoPickerModal />)} variant={AppButtonVariant.secondary} size={AppButtonSize.md}/>
                <AppButton title="Add Text" onClick={() => { } } variant={AppButtonVariant.secondary} size={AppButtonSize.md}/>                
            </div>            
        </div>         */}
        {editor && (
            <>
                <div className='flex gap-2 p-2 border-b border-white/40 bg-black/20'>
                    <button
                        type="button"
                        className={`px-2 py-1 text-sm rounded ${editor.isActive('bold') ? 'bg-white text-black' : 'bg-black/40 text-white'}`}
                        onClick={() => editor.chain().focus().toggleBold().run()}
                    >
                        Bold
                    </button>
                    <button
                        type="button"
                        className={`px-2 py-1 text-sm rounded ${editor.isActive('italic') ? 'bg-white text-black' : 'bg-black/40 text-white'}`}
                        onClick={() => editor.chain().focus().toggleItalic().run()}
                    >
                        Italic
                    </button>
                    <button
                        type="button"
                        className={`px-2 py-1 text-sm rounded ${editor.isActive('heading', { level: 2 }) ? 'bg-white text-black' : 'bg-black/40 text-white'}`}
                        onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
                    >
                        H2
                    </button>
                    <button
                        type="button"
                        className={`px-2 py-1 text-sm rounded ${editor.isActive('bulletList') ? 'bg-white text-black' : 'bg-black/40 text-white'}`}
                        onClick={() => editor.chain().focus().toggleBulletList().run()}
                    >
                        Bullets
                    </button>
                </div>
                <EditorContent
                    editor={editor}
                    className='flex-1 w-full h-full bg-transparent text-white p-4 overflow-auto'
                />
            </>
        )}
        
    </div>

    <AppButton title="Save" className='self-center' onClick={() => { }} variant={AppButtonVariant.go} size={AppButtonSize.md}/>
    </div>
  )
}

export default Editor_ImageCenter_OverlayCenterText