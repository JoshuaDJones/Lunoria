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
import { CharacterDto } from "../../types/character";
import CharacterTypeSelect, {
  CharacterType,
  ToCharacterType,
} from "../inputs/CharacterTypeSelect";
import { useLoading } from "../../providers/LoadingProvider";

interface CharacterForm {
  name: string;
  description: string;
  photo: File | undefined;
  maxHp: number;
  maxMp: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxInventory: number;
  isPlayer: boolean;
  isNPC: boolean;
  isEnemy: boolean;
}

interface AddEditCharacterModalProps {
  character?: CharacterDto;
  onSave: () => void;
}

const AddEditCharacterModal = ({
  character,
  onSave,
}: AddEditCharacterModalProps) => {
  const isEditMode = !!character;
  const modalRouter = useModalRouter();
  const { showToast } = useToast();
  const { showLoading, closeLoading } = useLoading();
  const { postForm, putForm } = useApi();

  const CharacterSchema = Yup.object().shape({
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
    maxHp: Yup.number().required("Max Hp is required."),
    maxMp: Yup.number().required("Max Mp is required."),
    meleeAttackDamage: Yup.number().nullable(),
    bowAttackDamage: Yup.number().nullable(),
    movement: Yup.number().required("Movement is required."),
    maxInventory: Yup.number().required("Max Inventory is required."),
    isPlayer: Yup.bool().required("Is Player is required."),
    isNPC: Yup.bool().required("Is NPC is required."),
    isEnemy: Yup.bool().required("Is Enemy is required."),
  });

  const handleCreateSubmit = async (values: CharacterForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (!values.photo || !(values.photo instanceof File)) return;
      formData.append("Photo", values.photo);

      formData.append("MaxHp", values.maxHp.toString());
      formData.append("MaxMp", values.maxMp.toString());

      if (values.meleeAttackDamage)
        formData.append(
          "MeleeAttackDamage",
          values.meleeAttackDamage.toString(),
        );

      if (values.bowAttackDamage)
        formData.append("BowAttackDamage", values.bowAttackDamage.toString());

      formData.append("Movement", values.movement.toString());
      formData.append("MaxInventory", values.maxInventory.toString());
      formData.append("IsPlayer", values.isPlayer.toString());
      formData.append("IsNPC", values.isNPC.toString());
      formData.append("IsEnemy", values.isEnemy.toString());

      await postForm(`${BASE_URL}/Character`, formData);

      showToast("Success:", "Character Created.", ToastType.success, 3000);
      onSave();
    } catch (err) {
      console.error("Error posting data:", err);
      showToast("Error:", "Issue creating Character.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  const handleEditSubmit = async (values: CharacterForm) => {
    try {
      showLoading();

      const formData = new FormData();

      formData.append("Name", values.name);
      formData.append("Description", values.description);

      if (values.photo && values.photo instanceof File) {
        formData.append("Photo", values.photo);
      }

      formData.append("MaxHp", values.maxHp.toString());
      formData.append("MaxMp", values.maxMp.toString());

      if (values.meleeAttackDamage)
        formData.append(
          "MeleeAttackDamage",
          values.meleeAttackDamage.toString(),
        );

      if (values.bowAttackDamage)
        formData.append("BowAttackDamage", values.bowAttackDamage.toString());

      formData.append("Movement", values.movement.toString());
      formData.append("MaxInventory", values.maxInventory.toString());
      formData.append("IsPlayer", values.isPlayer.toString());
      formData.append("IsNPC", values.isNPC.toString());
      formData.append("IsEnemy", values.isEnemy.toString());

      await putForm(`${BASE_URL}/Character/${character?.id}`, formData);

      showToast("Success:", "Character Edited.", ToastType.success, 3000);

      onSave();
    } catch (err) {
      console.error("Error putting data:", err);
      showToast("Error:", "Issue editing Character.", ToastType.error, 3000);
    } finally {
      modalRouter.pop();
      closeLoading();
    }
  };

  return (
    <AppModal onBackgroundClose={modalRouter.pop}>
      <RightModalContent
        title={isEditMode ? "Edit Character" : "Create Character"}
      >
        <Formik<CharacterForm>
          initialValues={
            isEditMode
              ? {
                  name: character.name,
                  description: character.description,
                  photo: undefined,
                  maxHp: character.maxHp,
                  maxMp: character.maxMp,
                  meleeAttackDamage: character.meleeAttackDamage,
                  bowAttackDamage: character.bowAttackDamage,
                  movement: character.movement,
                  maxInventory: character.maxInventory,
                  isPlayer: character.isPlayer,
                  isNPC: character.isNPC,
                  isEnemy: character.isEnemy,
                }
              : {
                  name: "",
                  description: "",
                  photo: undefined,
                  maxHp: 0,
                  maxMp: 0,
                  meleeAttackDamage: 0,
                  bowAttackDamage: 0,
                  movement: 0,
                  maxInventory: 0,
                  isPlayer: false,
                  isNPC: true,
                  isEnemy: false,
                }
          }
          validationSchema={CharacterSchema}
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
                        src={character.photoUrl}
                        className="w-[40%] object-cover rounded-md mt-1"
                      />
                    </div>
                  )}
                </div>

                <Field
                  name="maxHp"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Max Hp"
                  containerClassName="mt-8"
                />
                <InputError name="maxHp" />

                <Field
                  name="maxMp"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Max Mp"
                  containerClassName="mt-8"
                />
                <InputError name="maxMp" />

                <Field
                  name="meleeAttackDamage"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Melee Attack Damage"
                  containerClassName="mt-8"
                />
                <InputError name="meleeAttackDamage" />

                <Field
                  name="bowAttackDamage"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Bow Attack Damage"
                  containerClassName="mt-8"
                />
                <InputError name="bowAttackDamage" />

                <Field
                  name="movement"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Movement"
                  containerClassName="mt-8"
                />
                <InputError name="movement" />

                <Field
                  name="maxInventory"
                  type="text"
                  as={AppInput}
                  theme="dark"
                  title="Max Inventory"
                  containerClassName="mt-8"
                />
                <InputError name="maxInventory" />

                <CharacterTypeSelect
                  type={ToCharacterType(
                    values.isPlayer,
                    values.isNPC,
                    values.isEnemy,
                  )}
                  onSelect={(type: CharacterType) => {
                    setFieldValue("isPlayer", type === CharacterType.player);
                    setFieldValue("isNPC", type === CharacterType.npc);
                    setFieldValue("isEnemy", type === CharacterType.enemy);
                  }}
                />
                <InputError name="isPlayer" />
                <InputError name="isNPC" />
                <InputError name="isEnemy" />
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

export default AddEditCharacterModal;
