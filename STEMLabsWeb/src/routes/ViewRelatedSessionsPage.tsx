import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import Divider from "@mui/material/Divider";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import type { IdDate } from "../types/IdDate.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import isStringPositiveInteger from "../helpers/isStringPositiveInteger.tsx";
import TitleWithBackButton from "../components/TitleWithBackButton.tsx";
import dateToFormattedString from "../helpers/dateToFormatedString.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

export default function ViewRelatedSessionsPage() {
  const { laboratoryId } = useParams();
  const [sessions, setSessions] = useState<IdDate[]>([]);
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!isStringPositiveInteger(laboratoryId)) {
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
      let response;
      if (user.role.toLowerCase() === "admin") {
        response = await axiosRequestWithAutoReauth(
          {
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/laboratories/${laboratoryId}/sessions`,
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
          setUser,
        );
      } else {
        response = await axiosRequestWithAutoReauth(
          {
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/related-laboratories/${laboratoryId}/sessions`,
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
          setUser,
        );
      }

      setSessions(
        response.data.map((session: any) => ({
          id: session.id,
          date: new Date(session.date),
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
      width="600px"
      height="800px"
      bgcolor="white"
      boxShadow={4}
      overflow="visible"
      alignItems="center"
    >
      <TitleWithBackButton to=".." title="Select a session's date and time" />
      <Box width="100%" overflow="auto">
        <List sx={{ width: "100%" }}>
          {sessions.map((session) => {
            return (
              <>
                <Divider />
                <ListItem key={session.id} disablePadding>
                  <ListItemButton
                    component={Link}
                    to={
                      session.id +
                      (user?.role.toLowerCase() == "student"
                        ? `/${user.uid}`
                        : "")
                    }
                  >
                    <ListItemText
                      primary={dateToFormattedString(session.date)}
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
