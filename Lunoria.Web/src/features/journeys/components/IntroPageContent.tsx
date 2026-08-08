import { useEffect } from "react";
import type { JSONContent } from "@tiptap/core";
import { EditorContent, useEditor } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import { Button } from "@/components/ui";

interface IntroPageContentProps {
  content: JSONContent;
  editable?: boolean;
  onChange?: (content: JSONContent) => void;
}

export function IntroPageContent({
  content,
  editable = false,
  onChange,
}: IntroPageContentProps) {
  const editor = useEditor({
    extensions: [StarterKit],
    content,
    editable,
    onUpdate: ({ editor: currentEditor }) =>
      onChange?.(currentEditor.getJSON()),
  });

  useEffect(() => {
    if (
      editor &&
      JSON.stringify(editor.getJSON()) !== JSON.stringify(content)
    ) {
      editor.commands.setContent(content);
    }
  }, [content, editor]);

  if (!editor) return null;

  return (
    <div>
      {editable && (
        <div className="mb-3 flex flex-wrap gap-2 border-b border-border pb-3">
          <Button
            size="sm"
            variant={editor.isActive("paragraph") ? "primary" : "secondary"}
            onClick={() => editor.chain().focus().setParagraph().run()}
          >
            Paragraph
          </Button>
          {[1, 2, 3].map((level) => (
            <Button
              key={level}
              size="sm"
              variant={
                editor.isActive("heading", { level }) ? "primary" : "secondary"
              }
              onClick={() =>
                editor
                  .chain()
                  .focus()
                  .toggleHeading({ level: level as 1 | 2 | 3 })
                  .run()
              }
            >
              H{level}
            </Button>
          ))}
          <Button
            size="sm"
            variant={editor.isActive("bold") ? "primary" : "secondary"}
            onClick={() => editor.chain().focus().toggleBold().run()}
          >
            Bold
          </Button>
          <Button
            size="sm"
            variant={editor.isActive("italic") ? "primary" : "secondary"}
            onClick={() => editor.chain().focus().toggleItalic().run()}
          >
            Italic
          </Button>
          <Button
            size="sm"
            variant={editor.isActive("bulletList") ? "primary" : "secondary"}
            onClick={() => editor.chain().focus().toggleBulletList().run()}
          >
            Bullets
          </Button>
          <Button
            size="sm"
            variant={editor.isActive("orderedList") ? "primary" : "secondary"}
            onClick={() => editor.chain().focus().toggleOrderedList().run()}
          >
            Numbered
          </Button>
        </div>
      )}
      <EditorContent
        editor={editor}
        className={`[&_.tiptap]:min-h-28 [&_.tiptap]:outline-none [&_h1]:text-4xl [&_h2]:text-3xl [&_h3]:text-2xl [&_ol]:list-decimal [&_ol]:pl-6 [&_p]:my-2 [&_ul]:list-disc [&_ul]:pl-6 ${
          editable
            ? "rounded-lg border border-border bg-surface-raised p-4"
            : "text-content"
        }`}
      />
    </div>
  );
}
