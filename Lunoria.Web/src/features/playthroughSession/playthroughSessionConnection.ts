import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

function getApiOrigin(): string {
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;
  if (!apiBaseUrl) throw new Error("VITE_API_BASE_URL is not defined");
  return new URL(apiBaseUrl, window.location.origin).origin;
}

export function createPlaythroughSessionConnection(
  token: string,
): HubConnection {
  const connection = new HubConnectionBuilder()
    .withUrl(`${getApiOrigin()}/hubs/playthrough`)
    .withAutomaticReconnect([0, 1_000, 3_000, 10_000])
    .configureLogging(LogLevel.Warning)
    .build();

  connection.onreconnected(() => {
    void connection.invoke("JoinPlaythrough", token);
  });

  return connection;
}
