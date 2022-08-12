#!/bin/bash

ansible-playbook -i gbtesting_host $1 && \
    # ansible-playbook -i dresden_host $1 && \
    ansible-playbook -i chemnitz_host $1 && \
    ansible-playbook -i vogtland_host $1 && \
    ansible-playbook -i leipzig_host $1