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
import { ItemDto } from "../../types/item";
import { useLoading } from "../../providers/LoadingProvider";

interface ItemForm {
  name: string;
  description: string;
  photo: File | undefined;
  hpEffect: number;
  mpEffect: number;
}

interface AddEditItemModalProps {
  item?: ItemDto;
  onSave: () => void;
}

const AddEditItemModal = ({ item, onSave }: AddEditItemModalProps) => {
  const isEditMode = !!item;
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const { postForm, putForm } = useApi();

  const ItemSchema = Yup.object().shape({
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
    hpEffect: Yup.number().required("Hp Effect is required."),
    mpEffect: Yup.number().required("Mp Effect is required."),
  });

  const handleCreateSubmit = async (values: ItemForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (!values.photo || !(values.photo instanceof File)) return;
      formData.append("Photo", values.photo);

      formData.append("HpEffect", values.hpEffect.toString());
      formData.append("MpEffect", values.mpEffect.toString());

      await postForm(`${BASE_URL}/Item`, formData);

      showToast("Success:", "Item Created.", ToastType.success, 3000);
      onSave();
    } catch (err) {
      console.error("Error posting data:", err);
      showToast("Error:", "Issue creating Item.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const handleEditSubmit = async (values: ItemForm) => {
    try {
      showLoading();
      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (values.photo && values.photo instanceof File) {
        formData.append("Photo", values.photo);
      }

      formData.append("HpEffect", values.hpEffect.toString());
      formData.append("MpEffect", values.mpEffect.toString());

      await putForm(`${BASE_URL}/Item/${item?.id}`, formData);

      showToast("Success:", "Item Edited.", ToastType.success, 3000);

      onSave();
    } catch (err) {
      console.error("Error putting data:", err);
      showToast("Error:", "Issue editing Item.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={isEditMode ? "Edit Item" : "Create Item"}>
        <Formik<ItemForm>
          initialValues={
            isEditMode
              ? {
                  name: item.name,
                  description: item.description,
                  photo: undefined,
                  hpEffect: item.hpEffect,
                  mpEffect: item.mpEffect,
                }
              : {
                  name: "",
                  description: "",
                  photo: undefined,
                  hpEffect: 0,
                  mpEffect: 0,
                }
          }
          validationSchema={ItemSchema}
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
                        src={item.photoUrl}
                        className="w-[40%] object-cover rounded-md mt-1"
                      />
                    </div>
                  )}
                </div>

                <Field
                  name="hpEffect"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Hp Effect"
                  containerClassName="mt-8"
                />
                <InputError name="hpEffect" />

                <Field
                  name="mpEffect"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Mp Effect"
                  containerClassName="mt-8"
                />
                <InputError name="mpEffect" />
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

export default AddEditItemModal;
