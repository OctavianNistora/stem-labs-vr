import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import type { IdName } from "../types/IdName.tsx";
import Divider from "@mui/material/Divider";
import { Link } from "react-router";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

export default function ViewRelatedLaboratoriesPage() {
  const [laboratories, setLaboratories] = useState<IdName[]>([]);
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    fetchLaboratories();
  }, []);

  async function fetchLaboratories() {
    if (!user) {
      return;
    }

    try {
      let response;
      if (user.role.toLowerCase() === "admin") {
        response = await axiosRequestWithAutoReauth<IdName[]>(
          {
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/laboratories/simplified`,
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
          setUser,
        );
      } else {
        response = await axiosRequestWithAutoReauth<IdName[]>(
          {
            method: "GET",
            url: `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/related-laboratories`,
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          },
          setUser,
        );
      }
      setLaboratories(response.data);
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
      overflow="auto"
      alignItems="center"
    >
      <Typography variant="h4" marginY={2}>
        Select a laboratory
      </Typography>
      <List id={"laboratories-list"} sx={{ width: "100%" }}>
        {laboratories.map((laboratory) => (
          <>
            <Divider />
            <ListItem
              key={laboratory.id}
              id={laboratory.id.toString()}
              disablePadding
            >
              <ListItemButton component={Link} to={laboratory.id.toString()}>
                <ListItemText primary={laboratory.name} />
              </ListItemButton>
            </ListItem>
          </>
        ))}
      </List>
    </Box>
  );
}
