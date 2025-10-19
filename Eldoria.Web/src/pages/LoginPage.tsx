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
import { LoginResponse } from "../types/auth";

interface LoginForm {
  email: string;
  password: string;
}

const LoginSchema = Yup.object().shape({
  email: Yup.string().required("Email is required."),
  password: Yup.string().required("Password is required."),
});

const LoginPage = () => {
  const { setAuthToken } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();
  const { post } = useApi();

  const handleSubmit = async (values: LoginForm) => {
    try {
      const response: LoginResponse = await post(`${BASE_URL}/Auth/login`, {
        Email: values.email,
        Password: values.password,
      });

      if (response.accessToken) {
        setAuthToken(response.accessToken);
        navigate("/Journeys");
      } else {
        showToast(
          "Error:",
          "Those credentials are invalid.",
          ToastType.error,
          3000,
        );
      }
    } catch (err) {
      showToast(
        "Error:",
        "Those credentials are invalid.",
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
          <Formik<LoginForm>
            initialValues={{ email: "", password: "" }}
            validationSchema={LoginSchema}
            onSubmit={handleSubmit}
          >
            {({ isSubmitting }) => (
              <FormContent>
                <Title size={TitleSize.large} color={TitleColor.stone800}>
                  Login
                </Title>

                <Field
                  name="email"
                  type="text"
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
                  containerClassName="mt-8"
                />
                <InputError name="password" />

                <div className="mt-16 flex justify-center items-center gap-4">
                  <AppButton
                    title="Login"
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
                    title="Register"
                    type="button"
                    variant={AppButtonVariant.secondary}
                    size={AppButtonSize.lg}
                    onClick={() => navigate("/Register")}
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

export default LoginPage;
