import {
  ListItemIcon,
  ListItemText,
  styled,
  TextField,
  type TextFieldProps,
} from "@mui/material";

export const ProfileStyledListItemIcon = styled(ListItemIcon)(({}) => ({
  "& svg": {
    fontSize: "2em",
  },
}));
export const ProfileStyledListItemText = styled(ListItemText)(({}) => ({
  "& .MuiTypography-root": {
    fontSize: "1.2em",
  },
}));
export const ProfileStyledTextField = (
  props: TextFieldProps & { readOnly?: boolean },
) => {
  const { readOnly, ...rest } = props;

  return (
    <TextField
      variant="standard"
      slotProps={
        readOnly
          ? {
              input: {
                readOnly: true,
                disableUnderline: true,
              },
              inputLabel: {
                shrink: true,
              },
            }
          : undefined
      }
      {...rest}
    />
  );
};
