import { Box, Drawer, List, ListItem, ListItemButton } from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import EmailIcon from "@mui/icons-material/Email";
import PasswordIcon from "@mui/icons-material/Password";
import { useState } from "react";
import {
  ProfileStyledListItemIcon,
  ProfileStyledListItemText,
} from "../components/ProfileStyledMUIComponents.tsx";
import ProfilePageProfileSection from "../components/page-sections/profile-page/ProfilePageProfileSection.tsx";
import ProfilePageEmailSection from "../components/page-sections/profile-page/ProfilePageEmailSection.tsx";
import ProfilePagePasswordSection from "../components/page-sections/profile-page/ProfilePagePasswordSection.tsx";

const drawerWidth = 200;

export default function ProfilePage() {
  const [pageSection, setPageSection] = useState("profile");

  function handleSectionChange(section: string) {
    setPageSection(section);
  }

  function sectionContent(pageSection: string) {
    switch (pageSection) {
      case "profile":
        return <ProfilePageProfileSection />;
      case "email":
        return <ProfilePageEmailSection />;
      case "password":
        return <ProfilePagePasswordSection />;
      default:
        return <ProfilePageProfileSection />;
    }
  }

  return (
    <Box
      id="profile-container"
      display="flex"
      bgcolor="white"
      padding={2}
      width="35%"
      height="50%"
      position="relative"
      boxShadow={3}
    >
      <Drawer
        variant="permanent"
        slotProps={{
          paper: { sx: { width: drawerWidth, position: "absolute" } },
        }}
        sx={{ width: drawerWidth }}
      >
        <List sx={{ fontSize: 12 }}>
          <ListItem key="profile" disablePadding>
            <ListItemButton onClick={() => handleSectionChange("profile")}>
              <ProfileStyledListItemIcon>
                <PersonIcon />
              </ProfileStyledListItemIcon>
              <ProfileStyledListItemText
                primary={
                  <Box
                    fontWeight={pageSection === "profile" ? "bold" : "normal"}
                  >
                    Profile
                  </Box>
                }
              />
            </ListItemButton>
          </ListItem>
          <ListItem key="email" disablePadding>
            <ListItemButton onClick={() => handleSectionChange("email")}>
              <ProfileStyledListItemIcon>
                <EmailIcon />
              </ProfileStyledListItemIcon>
              <ProfileStyledListItemText
                primary={
                  <Box fontWeight={pageSection === "email" ? "bold" : "normal"}>
                    Email
                  </Box>
                }
              />
            </ListItemButton>
          </ListItem>
          <ListItem key="password" disablePadding>
            <ListItemButton onClick={() => handleSectionChange("password")}>
              <ProfileStyledListItemIcon>
                <PasswordIcon />
              </ProfileStyledListItemIcon>
              <ProfileStyledListItemText
                primary={
                  <Box
                    fontWeight={pageSection === "password" ? "bold" : "normal"}
                  >
                    Password
                  </Box>
                }
              />
            </ListItemButton>
          </ListItem>
        </List>
      </Drawer>
      <Box
        display="flex"
        width="100%"
        height="100%"
        justifyContent="center"
        alignItems="center"
      >
        {sectionContent(pageSection)}
      </Box>
    </Box>
  );
}
