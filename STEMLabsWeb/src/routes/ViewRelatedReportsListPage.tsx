import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import axios from "axios";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import TitleWithBackButton from "../components/TitleWithBackButton.tsx";
import Divider from "@mui/material/Divider";
import type { IdNameDate } from "../types/IdNameDate.tsx";

export default function ViewRelatedReportsListPage() {
  const { sessionId } = useParams();
  const [participants, setParticipants] = useState<IdNameDate[]>([]);
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!isStringPositiveInteger(sessionId) || user?.role === "student") {
      navigate("..");
      return;
    }

    fetchSessions();
  }, []);

  async function fetchSessions() {
    if (!user) {
      return;
    }

    try {
      const response = await axios.get(
        `${import.meta.env.VITE_API_URL}/api/laboratory-sessions/${sessionId}/participants`,
        {
          headers: {
            Authorization: `Bearer ${user.accessToken}`,
          },
        },
      );

      setParticipants(
        response.data.map((participant: any) => ({
          id: participant.id,
          name: participant.name,
          date: new Date(participant.date),
        })),
      );
    } catch (error) {
      toastErrorMessageHandle(addToast, setUser, error);
    }
  }

  return (
    <Box
      display="flex"
      flexDirection="column"
      width="800px"
      height="800px"
      bgcolor="white"
      boxShadow={4}
      overflow="visible"
      alignItems="center"
    >
      <TitleWithBackButton to=".." title="Select a participant" />
      <Box width="100%" overflow="auto">
        <List sx={{ width: "100%" }}>
          {participants.map((session) => {
            const sessionDate = session.date;
            const date =
              sessionDate.getDate().toString().padStart(2, "0") +
              "/" +
              (sessionDate.getMonth() + 1).toString().padStart(2, "0") +
              "/" +
              sessionDate.getFullYear();
            const time =
              sessionDate.getHours().toString().padStart(2, "0") +
              ":" +
              sessionDate.getMinutes().toString().padStart(2, "0") +
              ":" +
              sessionDate.getSeconds().toString().padStart(2, "0") +
              " UTC" +
              (sessionDate.getTimezoneOffset() < 0 ? "+" : "-") +
              sessionDate.getTimezoneOffset() / -60;
            const text = date + ", " + time;

            return (
              <>
                <Divider />
                <ListItem key={session.id} disablePadding>
                  <ListItemButton
                    component={Link}
                    to={
                      session.id +
                      (user?.role == "student" ? `/${user.uid}` : "")
                    }
                  >
                    <ListItemText primary={session.name} />
                    <ListItemText
                      primary={<Box textAlign="right">{text}</Box>}
                    />
                  </ListItemButton>
                </ListItem>
              </>
            );
          })}
        </List>
      </Box>
    </Box>
  );
}
