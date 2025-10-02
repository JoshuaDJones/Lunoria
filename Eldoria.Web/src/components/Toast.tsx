import Text, { TextSize } from "./typography/Text";
import { faTriangleExclamation } from "@fortawesome/free-solid-svg-icons";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { faCheck } from "@fortawesome/free-solid-svg-icons";
import { faClose } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import clsx from "clsx";
import "../styles/animations.css";
import { useEffect, useRef, useState } from "react";
import { ToastDuration, ToastType } from "../providers/ToastProvider";

interface ToastProps {
  id: string;
  title: string;
  message: string;
  type: ToastType;
  duration: ToastDuration;
  onDismiss: (id: string) => void;
}

const Toast = ({
  id,
  title,
  message,
  type,
  duration,
  onDismiss,
}: ToastProps) => {
  const [animation, setAnimation] = useState("fade-in-box");
  const timeoutRef = useRef<ReturnType<typeof setTimeout>>();

  let icon;
  let borderColor;
  let backgroundColor;

  useEffect(() => {
    timeoutRef.current = setTimeout(() => {
      handleDismiss();
    }, duration);

    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
    };
  }, [duration]);

  const handleDismiss = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current); // stop auto-dismiss if manually closing
    setAnimation("fade-out-box");
    setTimeout(() => {
      onDismiss(id);
    }, 1000); // matches fade-out duration
  };

  const getToastStyles = () => {
    switch (type) {
      case ToastType.success:
        icon = <FontAwesomeIcon icon={faCheck} size="2xl" color="#1f2937" />;
        borderColor = "border-green-400";
        backgroundColor = "bg-green-400";
        break;
      case ToastType.informational:
        icon = (
          <FontAwesomeIcon icon={faCircleInfo} size="2xl" color="#1f2937" />
        );
        borderColor = "border-blue-400";
        backgroundColor = "bg-blue-400";
        break;
      default:
      case ToastType.error:
        icon = (
          <FontAwesomeIcon
            icon={faTriangleExclamation}
            size="2xl"
            color="#1f2937"
          />
        );
        borderColor = "border-red-400";
        backgroundColor = "bg-red-400";
        break;
    }
  };

  getToastStyles();

  return (
    <div
      className={clsx(
        "flex max-w-[90%] md:max-w-[90%] w-[500px] rounded-lg bg-white shadow dark:bg-gray-700 border-l-8 gap-2",
        animation,
        borderColor,
      )}
    >
      <div
        className={clsx(
          "flex items-center justify-center pl-2 pr-4",
          backgroundColor,
        )}
      >
        {icon}
      </div>
      <div className="flex flex-col my-4 mx-2 flex-1">
        <Text className="font-bold" size={TextSize.xl}>
          {title}
        </Text>
        <Text>{message}</Text>
      </div>
      <button
        className="flex items-center justify-start h-full mt-2 mr-2 hover:opacity-80"
        onClick={handleDismiss}
      >
        <FontAwesomeIcon
          icon={faClose}
          size="2xl"
          className="text-black dark:text-white"
        />
      </button>
    </div>
  );
};

export default Toast;
