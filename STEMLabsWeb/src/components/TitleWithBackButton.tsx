import { Box, IconButton, Typography } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { Link, type To } from "react-router";

type TitleWithBackButtonProps = {
  to: To;
  title: string;
};
export default function TitleWithBackButton(props: TitleWithBackButtonProps) {
  const { to, title } = props;

  return (
    <Box width="100%" display="flex">
      <Box width={0} height={40} display="flex" flexDirection="row-reverse">
        <IconButton component={Link} to={to}>
          <ArrowBackIcon />
        </IconButton>
      </Box>
      <Box width="100%" position="relative">
        <Typography variant="h4" marginY={2} textAlign="center">
          {title}
        </Typography>
      </Box>
    </Box>
  );
}
