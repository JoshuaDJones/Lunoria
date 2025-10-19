import { useNavigate } from "react-router-dom";
import * as Yup from "yup";
import { Field, Formik } from "formik";
import { useAuth } from "../providers/AuthProvider";
import InputError from "../components/InputError";
import AppPage from "../components/layout/AppPage";
import PageContent from "../components/layout/PageContent";
import Title, { TitleColor, TitleSize } from "../components/typography/Title";
import AppInput from "../components/inputs/AppInput";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../components/buttons/AppButton";
import FormContent from "../components/layout/FormContent";
import { ToastType, useToast } from "../providers/ToastProvider";
import { BASE_URL, useApi } from "../hooks/useApi";

interface RegisterForm {
  email: string;
  password: string;
  confirmPassword: string;
}

const RegisterSchema = Yup.object().shape({
  email: Yup.string()
    .email("You must enter a valid email address.")
    .required("Email Address is required."),
  password: Yup.string()
    .min(8, "Password must have between 8 and 20 characters.")
    .max(20, "Password must have between 8 and 20 characters.")
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
      "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.",
    )
    .required("Password is required."),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("password")], "Passwords must match")
    .required("Confirm Password is required."),
});

const RegisterPage = () => {
  const { setAuthToken } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();
  const { post } = useApi();

  const handleSubmit = async (values: RegisterForm) => {
    try {
      const data = await post(`${BASE_URL}/Auth/register`, {
        Email: values.email,
        Password: values.password,
      });

      const token = data.token;

      if (token) {
        setAuthToken(token);
        showToast(
          "Success:",
          "Your account was successfully created.",
          ToastType.success,
          3000,
        );
        navigate("/");
      } else {
        showToast(
          "Error:",
          "There was an issue creating your account.",
          ToastType.error,
          3000,
        );
      }
    } catch (err) {
      showToast(
        "Error:",
        "There was an issue creating your account.",
        ToastType.error,
        3000,
      );
      console.error("Error posting data:", err);
    }
  };

  return (
    <AppPage backgroundImage="/Landing_Page.png">
      <PageContent>
        <div className="flex items-center justify-center">
          <Formik<RegisterForm>
            initialValues={{
              email: "",
              password: "",
              confirmPassword: "",
            }}
            validationSchema={RegisterSchema}
            onSubmit={handleSubmit}
          >
            {({ isSubmitting }) => (
              <FormContent>
                <Title size={TitleSize.large} color={TitleColor.stone800}>
                  Register
                </Title>

                <Field
                  name="email"
                  type="email"
                  as={AppInput}
                  title="Email"
                  containerClassName="mt-8"
                />
                <InputError name="email" />

                <Field
                  name="password"
                  type="password"
                  as={AppInput}
                  title="Password"
                  containerClassName="mt-6"
                />
                <InputError name="password" />

                <Field
                  name="confirmPassword"
                  type="password"
                  as={AppInput}
                  title="Confirm Password"
                  containerClassName="mt-6"
                />
                <InputError name="confirmPassword" />

                <div className="mt-16 flex justify-center items-center gap-4">
                  <AppButton
                    title="Register"
                    variant={
                      !isSubmitting
                        ? AppButtonVariant.primary
                        : AppButtonVariant.disabled
                    }
                    size={AppButtonSize.lg}
                    disabled={isSubmitting}
                    type="submit"
                  />
                  <AppButton
                    title="Back to Login"
                    variant={AppButtonVariant.secondary}
                    size={AppButtonSize.lg}
                    onClick={() => navigate("/login")}
                  />
                </div>
              </FormContent>
            )}
          </Formik>
        </div>
      </PageContent>
    </AppPage>
  );
};

export default RegisterPage;
