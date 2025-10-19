import { useEffect, useRef } from "react";
import Text, { TextColor, TextSize } from "../typography/Text";

export enum NotificationType {
  hp,
  mp,
}

export interface CardNotification {
  id: string;
  text: string;
  type: NotificationType;
  isVisible: boolean;
}

interface NotificationsListProps {
  notifications: CardNotification[];
  onHideNotification: (id: string) => void;
}

const NotificationsList = ({
  notifications,
  onHideNotification,
}: NotificationsListProps) => {
  return (
    <div className="absolute top-5 right-7 flex flex-col gap-2">
      {notifications.map((n) => (
        <NotificationItem
          key={n.id}
          notification={n}
          onHideNotification={onHideNotification}
        />
      ))}
    </div>
  );
};

const NotificationItem = ({
  notification,
  onHideNotification,
}: {
  notification: CardNotification;
  onHideNotification: (id: string) => void;
}) => {
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (!notification.isVisible) return;

    timeoutRef.current = setTimeout(() => {
      onHideNotification(notification.id);
      timeoutRef.current = null;
    }, 2000);

    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
        timeoutRef.current = null;
      }
    };
  }, [notification.id, notification.isVisible, onHideNotification]);

  if (!notification.isVisible) return null;

  return (
    <Text
      size={TextSize.lg}
      className="font-bold bg-gray-800/90 p-2 rounded-2xl"
      textColor={
        notification.type === NotificationType.hp
          ? TextColor.red
          : TextColor.green
      }
    >
      {notification.text}
    </Text>
  );
};

export default NotificationsList;
