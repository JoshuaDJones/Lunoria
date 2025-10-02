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
import { useLoading } from "../../providers/LoadingProvider";
import { SpellDto } from "../../types/spell";
import CheckInput from "../inputs/CheckInput";

interface SpellForm {
  name: string;
  description: string;
  photo: File | undefined;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect: number | null;
  healthEffect: number | null;
  magicEffect: number | null;
}

interface AddEditSpellModalProps {
  spell?: SpellDto;
  onSave: () => void;
}

const AddEditSpellModal = ({ spell, onSave }: AddEditSpellModalProps) => {
  const isEditMode = !!spell;
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const { postForm, putForm } = useApi();

  const SpellSchema = Yup.object().shape({
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
    range: Yup.number().required("Range is required."),
    isRadius: Yup.bool().required("IsRadius is required."),
    mpCost: Yup.number().required("MpCost is required"),
    damageEffect: Yup.number().nullable(),
    healthEffect: Yup.number().nullable(),
    magicEffect: Yup.number().nullable(),
  });

  const handleCreateSubmit = async (values: SpellForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (!values.photo || !(values.photo instanceof File)) return;
      formData.append("Photo", values.photo);

      formData.append("Range", values.range.toString());
      formData.append("IsRadius", values.isRadius.toString());
      formData.append("MpCost", values.mpCost.toString());

      if (values.damageEffect)
        formData.append("DamageEffect", values.damageEffect.toString());

      if (values.healthEffect)
        formData.append("HealthEffect", values.healthEffect.toString());

      if (values.magicEffect)
        formData.append("MagicEffect", values.magicEffect.toString());

      await postForm(`${BASE_URL}/Spell`, formData);

      showToast("Success:", "Spell Created.", ToastType.success, 3000);
      onSave();
    } catch (err) {
      console.error("Error posting data:", err);
      showToast("Error:", "Issue creating Spell.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const handleEditSubmit = async (values: SpellForm) => {
    try {
      showLoading();
      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (values.photo && values.photo instanceof File) {
        formData.append("Photo", values.photo);
      }

      formData.append("Range", values.range.toString());
      formData.append("IsRadius", values.isRadius.toString());
      formData.append("MpCost", values.mpCost.toString());

      if (values.damageEffect)
        formData.append("DamageEffect", values.damageEffect.toString());

      if (values.healthEffect)
        formData.append("HealthEffect", values.healthEffect.toString());

      if (values.magicEffect)
        formData.append("MagicEffect", values.magicEffect.toString());

      await putForm(`${BASE_URL}/Spell/${spell?.id}`, formData);

      showToast("Success:", "Spell Edited.", ToastType.success, 3000);

      onSave();
    } catch (err) {
      console.error("Error putting data:", err);
      showToast("Error:", "Issue editing Spell.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent title={isEditMode ? "Edit Spell" : "Create Spell"}>
        <Formik<SpellForm>
          initialValues={
            isEditMode
              ? {
                  name: spell.name,
                  description: spell.description,
                  photo: undefined,
                  range: spell.range,
                  isRadius: spell.isRadius,
                  mpCost: spell.mpCost,
                  damageEffect: spell.damageEffect,
                  healthEffect: spell.healthEffect,
                  magicEffect: spell.magicEffect,
                }
              : {
                  name: "",
                  description: "",
                  photo: undefined,
                  range: 0,
                  isRadius: false,
                  mpCost: 0,
                  damageEffect: 0,
                  healthEffect: 0,
                  magicEffect: 0,
                }
          }
          validationSchema={SpellSchema}
          onSubmit={isEditMode ? handleEditSubmit : handleCreateSubmit}
        >
          {({ isSubmitting, setFieldValue, values }) => (
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
                        src={spell.photoUrl}
                        className="w-[40%] object-cover rounded-md mt-1"
                      />
                    </div>
                  )}
                </div>

                <Field
                  name="range"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Range"
                  containerClassName="mt-8"
                />
                <InputError name="range" />

                <CheckInput
                  title={"IsRadius"}
                  isSelected={values.isRadius}
                  onChange={(isSelected) =>
                    setFieldValue("isRadius", isSelected)
                  }
                  className="mt-8"
                />
                <InputError name="isRadius" />

                <Field
                  name="mpCost"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Mp Cost"
                  containerClassName="mt-8"
                />
                <InputError name="mpCost" />

                <Field
                  name="damageEffect"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Damage Effect"
                  containerClassName="mt-8"
                />
                <InputError name="damageEffect" />

                <Field
                  name="healthEffect"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Health Effect"
                  containerClassName="mt-8"
                />
                <InputError name="healthEffect" />

                <Field
                  name="magicEffect"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Magic Effect"
                  containerClassName="mt-8"
                />
                <InputError name="magicEffect" />
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

export default AddEditSpellModal;
