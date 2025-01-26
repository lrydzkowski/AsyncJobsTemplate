/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import { useState } from 'react';
import { IconChevronRight } from '@tabler/icons-react';
import { NavLink } from 'react-router';
import { Box, Collapse, Group, Text, UnstyledButton } from '@mantine/core';
import classes from './navbar-links-group.module.css';

interface LinksGroupProps {
  label: string;
  currentLink?: string;
  initiallyOpened?: boolean;
  links?: { label: string; link: string }[];
}

export function LinksGroup({ label, currentLink, initiallyOpened, links }: LinksGroupProps) {
  const hasLinks = Array.isArray(links);
  const [opened, setOpened] = useState(initiallyOpened ?? false);
  const items = (hasLinks ? links : []).map((link) => (
    <NavLink to={link.link} key={link.label} className={`${classes.link} mantine-focus-auto`}>
      {link.label}
    </NavLink>
  ));

  return (
    <>
      {currentLink ? (
        <NavLink to={currentLink} key={label} className={`${classes.topLink} mantine-focus-auto`}>
          <Text ml="1rem" fw="500">
            {label}
          </Text>
        </NavLink>
      ) : (
        <>
          <UnstyledButton onClick={() => setOpened((o) => !o)} className={classes.control}>
            <Group justify="space-between" gap={0}>
              <Box style={{ display: 'flex', alignItems: 'center' }}>
                <Box ml="md">{label}</Box>
              </Box>
              {hasLinks && (
                <IconChevronRight
                  className={classes.chevron}
                  stroke={1.5}
                  size={16}
                  style={{ transform: opened ? 'rotate(-90deg)' : 'none' }}
                />
              )}
            </Group>
          </UnstyledButton>
          {hasLinks ? <Collapse in={opened}>{items}</Collapse> : null}
        </>
      )}
    </>
  );
}
