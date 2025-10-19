import { SceneDto } from "../../types/scene";
import { useModalRouter } from "../../providers/ModalRouterProvider";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { useLoading } from "../../providers/LoadingProvider";
import { ToastType, useToast } from "../../providers/ToastProvider";
import * as Yup from "yup";
import AppModal from "./AppModal";
import RightModalContent from "./RightModalContent";
import { Field, Form, Formik } from "formik";
import InputError from "../InputError";
import AppInput from "../inputs/AppInput";
import FileInput from "../inputs/FileInput";
import Text, { TextColor } from "../typography/Text";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";

interface SceneForm {
  name: string;
  description: string;
  photo: File | undefined;
  gridUrl: string;
}

interface AddEditSceneModalProps {
  journeyId: number;
  scene?: SceneDto;
  onSave: () => void;
}

const AddEditSceneModal = ({
  journeyId,
  scene,
  onSave,
}: AddEditSceneModalProps) => {
  const isEditMode = !!scene;
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const { postForm, putForm } = useApi();

  const SceneSchema = Yup.object().shape({
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
    gridUrl: Yup.string().required("Grid URL is required."),
  });

  const handleCreateSubmit = async (values: SceneForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("JourneyId", journeyId.toString());
      formData.append("Name", values.name);
      formData.append("Description", values.description);
      formData.append("GridUrl", values.gridUrl);

      if (!values.photo || !(values.photo instanceof File)) return;
      formData.append("Photo", values.photo);

      await postForm(`${BASE_URL}/Scene`, formData);

      showToast("Success:", "Scene Created.", ToastType.success, 3000);
      onSave();
    } catch (err) {
      console.error("Error posting data:", err);
      showToast("Error:", "Issue creating Scene.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const handleEditSubmit = async (values: SceneForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("JourneyId", values.name);
      formData.append("Name", values.name);
      formData.append("Description", values.description);
      formData.append("GridUrl", values.gridUrl);

      if (values.photo && values.photo instanceof File) {
        formData.append("Photo", values.photo);
      }

      await putForm(`${BASE_URL}/Scene/${scene?.id}`, formData);

      showToast("Success:", "Scene Edited.", ToastType.success, 3000);

      onSave();
    } catch (err) {
      console.error("Error putting data:", err);
      showToast("Error:", "Issue editing Scene.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={isEditMode ? "Edit Scene" : "Create Scene"}>
        <Formik<SceneForm>
          initialValues={
            isEditMode
              ? {
                  name: scene.name,
                  description: scene.description,
                  photo: undefined,
                  gridUrl: scene.gridUrl,
                }
              : {
                  name: "",
                  description: "",
                  photo: undefined,
                  gridUrl: "",
                }
          }
          validationSchema={SceneSchema}
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
                        src={scene.photoUrl}
                        className="w-[40%] object-cover rounded-md mt-1"
                      />
                    </div>
                  )}
                </div>

                <Field
                  name="gridUrl"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Grid URL"
                  containerClassName="mt-8"
                />
                <InputError name="gridUrl" />
              </div>

              <AppButton
                title={"Save"}
                variant={AppButtonVariant.primary}
                size={AppButtonSize.lg}
                disabled={isSubmitting}
                type="submit"
                className="self-center mt-5"
              />
            </Form>
          )}
        </Formik>
      </RightModalContent>
    </AppModal>
  );
};

export default AddEditSceneModal;
