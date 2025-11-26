<?php
// Auto-connect configuration for phpPgAdmin

$conf['servers'][0]['desc'] = 'PostgreSQL AutoConnect';
$conf['servers'][0]['host'] = 'postgreshost';      // matches the service name
$conf['servers'][0]['port'] = 5432;
$conf['servers'][0]['sslmode'] = 'disable';
$conf['servers'][0]['defaultdb'] = 'db';
$conf['servers'][0]['pg_dump_path'] = '/usr/bin/pg_dump';
$conf['servers'][0]['pg_dumpall_path'] = '/usr/bin/pg_dumpall';

// Auto-login user (DISABLED LOGIN SCREEN)
$conf['servers'][0]['user'] = 'exampleuser';
$conf['servers'][0]['password'] = 'pass';
$conf['extra_login_security'] = false;

// Hide the login form entirely
$conf['owned_only'] = false;
$conf['show_advanced'] = true;
?>