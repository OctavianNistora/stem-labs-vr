import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  Typography,
} from "@mui/material";
import { getDrawerList } from "../layouts/DrawerLayout";
import { useContext } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { Link } from "react-router";

export default function HomePage() {
  const { user } = useContext(AuthContext);

  return (
    <Box
      width="100%"
      height="100%"
      display="flex"
      flexDirection="column"
      justifyContent="center"
      alignContent="center"
      alignItems="center"
      gap={5}
    >
      <Typography variant="h3" sx={{ fontWeight: "bold" }}>
        Welcome to STEM Labs VR
      </Typography>
      <List component={Stack} direction="row">
        {getDrawerList(user?.role).map((item) => (
          <ListItem alignItems="flex-start" key={item.text}>
            <ListItemButton component={Link} to={item.link}>
              <Stack alignItems="center" alignContent="center">
                <ListItemIcon
                  sx={{
                    "& svg": {
                      fontSize: "6em",
                      bgcolor: "white",
                      borderRadius: "10%",
                      boxShadow: 2,
                    },
                  }}
                >
                  {item.icon}
                </ListItemIcon>
                <ListItemText
                  slotProps={{ primary: { fontSize: "1em" } }}
                  primary={<Box textAlign="center">{item.text}</Box>}
                />
              </Stack>
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );
}
