import AppModal from "./AppModal";
import RightModalContent from "./RightModalContent";
import Text, { TextColor } from "../typography/Text";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import * as Yup from "yup";
import { Field, Form, Formik } from "formik";
import AppInput from "../inputs/AppInput";
import InputError from "../InputError";
import FileInput from "../inputs/FileInput";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { ToastType, useToast } from "../../providers/ToastProvider";
import { JourneyDto } from "../../types/journey";

interface JourneyForm {
  name: string;
  description: string;
  photo: File | undefined;
}

interface AddEditJourneyModalProps {
  journey?: JourneyDto;
  onSave: () => void;
}

const AddEditJourneyModal = ({ journey, onSave }: AddEditJourneyModalProps) => {
  const isEditMode = !!journey;
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { postForm, putForm } = useApi();

  const JourneySchema = Yup.object().shape({
    name: Yup.string().required("Name is required."),
    description: Yup.string().required("Description is required."),
    photo: isEditMode
      ? Yup.mixed<File>()
          .nullable()
          .test("fileType", "Unsupported file format.", (value) =>
            value ? ["image/jpeg", "image/png"].includes(value.type) : true,
          )
          .test("fileSize", "File too large. Max 5MB.", (value) =>
            value ? value.size <= 5 * 1024 * 1024 : true,
          )
      : Yup.mixed<File>()
          .required("Photo is required.")
          .test("fileType", "Unsupported file format.", (value) =>
            value ? ["image/jpeg", "image/png"].includes(value.type) : false,
          )
          .test("fileSize", "File too large. Max 5MB.", (value) =>
            value ? value.size <= 5 * 1024 * 1024 : false,
          ),
  });

  const handleCreateSubmit = async (values: JourneyForm) => {
    try {
      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (!values.photo || !(values.photo instanceof File)) return;
      formData.append("Photo", values.photo);

      await postForm(`${BASE_URL}/Journey`, formData);

      showToast("Success:", "Journey Created.", ToastType.success, 3000);
      onSave();
    } catch (err) {
      console.error("Error posting data:", err);
      showToast("Error:", "Issue creating Journey.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
    }
  };

  const handleEditSubmit = async (values: JourneyForm) => {
    try {
      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (values.photo && values.photo instanceof File) {
        formData.append("Photo", values.photo);
      }

      await putForm(`${BASE_URL}/Journey/${journey?.id}`, formData);

      showToast("Success:", "Journey Edited.", ToastType.success, 3000);

      onSave();
    } catch (err) {
      console.error("Error putting data:", err);
      showToast("Error:", "Issue editing Journey.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={isEditMode ? "Edit Journey" : "Create Journey"}>
        <Formik<JourneyForm>
          initialValues={
            isEditMode
              ? {
                  name: journey.name,
                  description: journey.description,
                  photo: undefined,
                }
              : {
                  name: "",
                  description: "",
                  photo: undefined,
                }
          }
          validationSchema={JourneySchema}
          onSubmit={isEditMode ? handleEditSubmit : handleCreateSubmit}
        >
          {({ isSubmitting, setFieldValue }) => (
            <Form className="flex flex-col flex-1">
              <div className="flex flex-col flex-1">
                <Field
                  name="name"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Name"
                  containerClassName="mt-8"
                />
                <InputError name="name" />

                <Field
                  name="description"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Description"
                  containerClassName="mt-8"
                />
                <InputError name="description" />

                <div className="flex flex-col mt-8">
                  <FileInput
                    title={"Photo"}
                    onFileSelect={(file) => setFieldValue("photo", file)}
                  />
                  <InputError name="photo" />
                  {isEditMode && (
                    <div className="flex flex-col mt-2 ml-4">
                      <Text
                        className="text-gray-400"
                        textColor={TextColor.custom}
                      >
                        Please Note: Adding an image will overwrite the image
                        that is already saved below.
                      </Text>
                      <img
                        src={journey.photoUrl}
                        className="w-[40%] object-cover rounded-md mt-1"
                      />
                    </div>
                  )}
                </div>
              </div>
              <AppButton
                title={"Save"}
                variant={AppButtonVariant.primary}
                size={AppButtonSize.lg}
                disabled={isSubmitting}
                type="submit"
                className="self-center"
              />
            </Form>
          )}
        </Formik>
      </RightModalContent>
    </AppModal>
  );
};

export default AddEditJourneyModal;
