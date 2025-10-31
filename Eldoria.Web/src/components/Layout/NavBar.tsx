import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { useLocation, useNavigate } from "react-router-dom";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import NavButton from "../buttons/NavButton";
import { useAuth } from "../../providers/AuthProvider";

interface NavBarProps {
  hasBackButton?: boolean;
}

const NavBar = ({ hasBackButton = false }: NavBarProps) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { setAuthToken } = useAuth();

  return (
    <div className="absolute top-0 right-0 left-0 flex items-center bg-stone-800/90 h-28 w-full z-20 border-b-4 border-stone-800 px-5">
      {hasBackButton && (
        <button
          type="button"
          className="hover:opacity-50 p-2"
          onClick={() => navigate(-1)}
        >
          <FontAwesomeIcon
            size="2x"
            icon={faArrowLeft}
            className="text-gray-300"
          />
        </button>
      )}

      <Title color={TitleColor.white} size={TitleSize.large}>
        Lunoria
      </Title>

      <div className="w-full justify-end flex gap-10">
        <NavButton
          title={"Journeys"}
          isSelected={location.pathname === "/Journeys"}
          onClick={() => navigate("/Journeys")}
        />
        <NavButton
          title={"Characters"}
          isSelected={location.pathname === "/Characters"}
          onClick={() => navigate("/Characters")}
        />
        <NavButton
          title={"Spells"}
          isSelected={location.pathname === "/Spells"}
          onClick={() => navigate("/Spells")}
        />
        <NavButton
          title={"Items"}
          isSelected={location.pathname === "/Items"}
          onClick={() => navigate("/Items")}
        />
        <NavButton
          title={"Logout"}
          isSelected={false}
          onClick={() => setAuthToken(undefined)}
        />
      </div>
    </div>
  );
};

export default NavBar;
